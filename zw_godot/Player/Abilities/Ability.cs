using Godot;
using System;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using Godot.Collections;


public partial class Ability : Node
{
	public bool Cast = false;
    public Camera3D MainCamera;
    private StaticBody3D Ground;
    private Node3D Main;
    private AbilityUI AbilityUI;

    PackedScene AreaIndicator;
    PackedScene ArrowIndicator;
    PackedScene ConeIndicator;

    PackedScene AreaHitbox;
    PackedScene ArrowHitbox;

    public CancellationTokenSource cancellationTokenSource;

    public bool Casting = false;
    public bool AttackMoving = false;

    private const float RayLength = 1000.0f;

    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;


    public Player Player;

    float CooldownReduction;

    int AutoAttacksLaunched;
    Godot.Timer AutoAttackEndTimer;

    public List<StatEffect> AttackSpeedEffects;

    public TaskCompletionSource<bool> AbilityCast = new();
    public TaskCompletionSource<bool> AttackMoveContinue = new();

    System.Timers.Timer AbilityTimer;
    public TaskCompletionSource<bool> AbilityTimerFinished = new();

    PackedScene StatEffect;

    public override void _Ready()
	{
        AreaIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/AreaIndicator.tscn");
        ArrowIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/ArrowIndicator.tscn");
        ConeIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/ConeIndicator.tscn");
        AreaHitbox = (PackedScene)ResourceLoader.Load("res://Player/Abilities/AreaHitbox.tscn");
        ArrowHitbox = (PackedScene)ResourceLoader.Load("res://Player/Abilities/ArrowHitbox.tscn");
        StatEffect = (PackedScene)ResourceLoader.Load("res://Player/Abilities/StatEffect.tscn");

        AutoAttackEndTimer = GetNode<Godot.Timer>("AutoAttackEndTimer");

        Player = GetParent<Player>();
        MainCamera = Player.MainCamera;
        Ground = Player.Ground;
        AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");

        Main = Player.GetParent<Node3D>();

        AttackSpeedEffects = new();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if ((Casting | Player.ClosestTarget != null) & AttackMoving) 
        {
            AttackMoveContinue.SetResult(true);
            AttackMoving = false;
        }

        CooldownReduction = 1 - (100 - (10000 / (100 + 2 * Player.AbilityHaste))) / 100;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left && Casting == true)
        {
            AbilityCast.SetResult(true);
        }

    }

    public virtual System.Collections.Generic.Dictionary<string, object> AbilityRaycast()
    {
        Vector2 mouse_pos = GetViewport().GetMousePosition();
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = MainCamera.ProjectRayOrigin(mouse_pos),
            To = MainCamera.ProjectRayOrigin(mouse_pos) + MainCamera.ProjectRayNormal(mouse_pos) * RayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = MouseColliderLayers,
        };

        var hitDictionary = Player.GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0)
        {
            var objectHit = hitDictionary["collider"].Obj;
            var positionHit = hitDictionary["position"].Obj; 

            if  (objectHit == Ground | objectHit is Enemy)
            {
                System.Collections.Generic.Dictionary<string, object> info = new()
                {
                    { "objectHit", objectHit },
                    { "positionHit", positionHit }
                };

                return info;
            }
        }
        return null;
    }

    public async void AttackMove()
    {
        // Change cursor
        AbilityCast.SetResult(false); // Cancel other abilities

        AbilityCast = new TaskCompletionSource<bool>(); // Reset task to await cast confirmation
        Casting = true;

        if (await AbilityCast.Task == true) // Ability casted
        {
            // Cursor goes back to normal
            Casting = false; // Reset casting boolean
            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            AttackMoveContinue = new();
            AttackMoveContinue.SetResult(false);
            AttackMoveContinue = new();
            AttackMoving = true;

            Vector3 movePoint = (Vector3)AbilityRaycast()["positionHit"];

            System.Collections.Generic.Dictionary<string, object> message = new()
            {
                {"MovePoint",  movePoint}
            };

            Player.PlayerStateMachine.ChangeState("MovingState", message);

            if (await AttackMoveContinue.Task == true)
            {

                if (Casting)
                {
                    GD.Print("AttackMove cancel because of alternate casting.");
                    AttackMoveContinue = new();
                    AttackMoving = false;
                    return;
                }

                else if (Player.ClosestTarget != null)
                {
                    GD.Print("Player entered detection range and starts attack");
                    System.Collections.Generic.Dictionary<string, object> message2 = new()
                    {
                        {"Target", Player.ClosestTarget},
                        {"Projectile Speed" ,  Player.ProjectileSpeed}
                    };
                    
                if (Player.RangedAttack)
                    Player.PlayerStateMachine.ChangeState("RangeAttackingState", message2);

                else
                    Player.PlayerStateMachine.ChangeState("AttackingState", message2);

                    AttackMoving = false;
                }
            }
            
            else
            {
                GD.Print("AttackMove cancel because of alternate casting of AttackMove.");
                AttackMoveContinue = new();
                AttackMoving = false;
                return;
            }
        }

        else // Ability canceled
        {
            // Cursor goes back to normal

            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Casting = false; // Reset casting boolean
            AttackMoveContinue = new();
            AttackMoving = false;
        }
    }

    public async void Warrior1(Entity caster)
    {
        /// <summary>Point and click massive damage.</summary>

        // Change cursor

        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();
        Casting = true;

        if (await AbilityCast.Task == true)
        {
            Node3D node = (Node3D)AbilityRaycast()["objectHit"];

            if (node != null)
            {
                if ((Node3D)AbilityRaycast()["objectHit"] is Enemy enemyHit)
                {
                    GD.Print("enemy found");

                    System.Collections.Generic.Dictionary<string, object> message = new()
                    {
                        {"Target",  enemyHit},
                        {"Ability", "Warrior1"},
                        {"Range", 2f},
                        {"DamageMultiplier", 2.5f},
                        {"Cooldown", 10 * CooldownReduction}
                    };
                    Player.PlayerStateMachine.ChangeState("AttackingState", message);
                }
            }

            // Cursor goes back to normal
            GD.Print("overstrike casted");
            AbilityCast = new TaskCompletionSource<bool>();
            Casting = false;

        }
        else
        {
            // Cursor goes back to normal
            GD.Print("overstrike cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
            Casting = false;
        }
    }

    public void Warrior2(Entity caster)
    {
        CreateStatEffect(5, "AttackSpeed", 0.5);
        CreateStatEffect(5, "Damage", 0.2);

        AbilityUI.SetAbilityCooldown("Ability2", 20 * CooldownReduction);
    }

    public async void Warrior3(Entity caster)
    {
        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();
        Casting = true;

        float length = 4;
        float width = 3;

        LineIndicator ConeIndic = (LineIndicator)ConeIndicator.Instantiate();
        ConeIndic.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ConeIndic);

        ArrowHitbox ConeHB = (ArrowHitbox)ArrowHitbox.Instantiate();
        ConeHB.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ConeHB);

        if (await AbilityCast.Task == true)
        {
            AbilityCast = new TaskCompletionSource<bool>();
            ConeIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Warrior 3 casted");

            // loop
            AbilityTimer = new();
            AbilityTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            AbilityTimer.AutoReset = true;
            AbilityTimer.Interval = 333;
            AbilityTimer.Enabled = true;

            Player.StatsBonusMult["MovementSpeed"] += -0.5;
            Player.UpdateStats();

            AbilityUI.SetAbilityCooldown("Ability3", 20 * CooldownReduction);
            int loopNumber = 0;
            ConeHB.PositionLocked = true;
            while (loopNumber != 6)
            {
                if (await AbilityTimerFinished.Task == true)
                {
                    Array<Node3D> targets = ConeHB.GetOverlappingBodies();
                    foreach (Entity target in targets)
                    {
                        Player.DealDamage(target, (int)Math.Round(Player.Damage * 0.66));
                    }

                    AbilityTimerFinished = new TaskCompletionSource<bool>();

                    loopNumber ++;
                }
            }
            Player.StatsBonusMult["MovementSpeed"] -= -0.5;
            Player.UpdateStats();

            AbilityTimer.Enabled = false;

            AbilityCast = new TaskCompletionSource<bool>();
        }
        else
        {
            ConeIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Cone cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
        }
    }

    public async void Warrior4(Entity caster)
    {
        /// <summary>When 10 aa have been used consecutively, jump on an enemy and deal big damage. Gain 5% attack speed until you drop the aa. No cooldown</summary>

        // Change cursor

        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();
        Casting = true;

        if (await AbilityCast.Task == true)
        {
            Node3D node = (Node3D)AbilityRaycast()["objectHit"];

            if (node != null)
            {
                if ((Node3D)AbilityRaycast()["objectHit"] is Enemy enemyHit)
                {
                    System.Collections.Generic.Dictionary<string, object> message = new()
                    {
                        {"Target",  enemyHit},
                        {"Ability", "Leap"},
                        {"Range", 2f},
                        {"Leap Range", 10f},
                        {"DamageMultiplier", 2f},
                        {"Cooldown", 10}
                    };
                    Player.PlayerStateMachine.ChangeState("AttackingState", message);
                }
            }

            // Cursor goes back to normal
            GD.Print("Leap casted");
            AbilityCast = new TaskCompletionSource<bool>();
            Casting = false;
        }

        else
        {
            // Cursor goes back to normal
            GD.Print("Leap cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
            Casting = false;
        }
    }

    public async void Flamestorm(Entity caster)
    {
        // Change cursor

        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();
        Casting = true;
        float radius = 2;

        AreaIndicator AreaIndic = (AreaIndicator)AreaIndicator.Instantiate();
        AreaIndic.Scale = 2 * radius * Vector3.One;
        Main.AddChild(AreaIndic);
        AreaHitbox AreaHB = (AreaHitbox)AreaHitbox.Instantiate();
        AreaHB.Scale = 2 * radius * Vector3.One;
        Main.AddChild(AreaHB);

        if (await AbilityCast.Task == true)
        {
            AreaIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Flamestorm casted");
            
            Array<Node3D> targets = AreaHB.GetOverlappingBodies();
            foreach (Entity target in targets)
            {
                caster.DealDamage(target, caster.Damage);
            }

            AreaHB.QueueFree();
            AbilityCast = new TaskCompletionSource<bool>();

            AbilityUI.SetAbilityCooldown("Ability2", 10);
            Player.PlayerStateMachine.ChangeState("IdleState");

        }
        else
        {
            AreaIndic.QueueFree();
            AreaHB.QueueFree();
            // Cursor goes back to normal
            GD.Print("Flamestorm cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
        }
    }

    public async void Arrowshot(Entity caster)
    {
        // Change cursor

        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();
        Casting = true;

        float length = 4;
        float width = 1;

        LineIndicator ArrowIndic = (LineIndicator)ArrowIndicator.Instantiate();
        ArrowIndic.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ArrowIndic);

        ArrowHitbox ArrowHB = (ArrowHitbox)ArrowHitbox.Instantiate();
        ArrowHB.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ArrowHB);

        if (await AbilityCast.Task == true)
        {
            // Cursor goes back to normal
            GD.Print("Arrowshot casted");

            Array<Node3D> targets = ArrowHB.GetOverlappingBodies();
            GD.Print(targets);
            foreach (Entity target in targets)
            {
                caster.DealDamage(target, caster.Damage);

                GD.Print("enemy hit with arrowshot");
            }

            ArrowIndic.QueueFree();
            ArrowHB.QueueFree();

            AbilityCast = new TaskCompletionSource<bool>();

            AbilityUI.SetAbilityCooldown("Ability3", 10);
            Player.PlayerStateMachine.ChangeState("IdleState");

        }
        else
        {
            ArrowIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Arrowshot cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
        }
    }

    public async void Cone(Entity caster)
    {
        // Change cursor

        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();
        Casting = true;

        float length = 4;
        float width = 3;

        LineIndicator ConeIndic = (LineIndicator)ConeIndicator.Instantiate();
        ConeIndic.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ConeIndic);

        ArrowHitbox ConeHB = (ArrowHitbox)ArrowHitbox.Instantiate();
        ConeHB.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ConeHB);

        if (await AbilityCast.Task == true)
        {
            ConeIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Cone casted");

            Array<Node3D> targets = ConeHB.GetOverlappingBodies();
            foreach (Entity target in targets)
            {
                caster.DealDamage(target, caster.Damage);
            }

            AbilityCast = new TaskCompletionSource<bool>();

            AbilityUI.SetAbilityCooldown("Ability4", 10);
            Player.PlayerStateMachine.ChangeState("IdleState");

        }
        else
        {
            ConeIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Cone cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
        }
    }

    public StatEffect CreateStatEffect(float duration, string statEffect, double effectAmount)
    {
        StatEffect effect = (StatEffect)StatEffect.Instantiate();
        effect.Player = Player;
        effect.Message = new()
        {
            {"Duration", duration},
            {"StatEffect", statEffect},
            {"EffectAmount", effectAmount}
        };

        Player.AddChild(effect);
        return effect;
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
        AbilityTimerFinished.SetResult(true);
    }

    public async void AbilityStructure()
    {
        // Change cursor

        AbilityCast.SetResult(false); // Cancel other abilities
        AbilityCast = new TaskCompletionSource<bool>(); // Reset task to await cast confirmation
        Casting = true;

        if (await AbilityCast.Task == true) // Ability casted
        {
            // Cursor goes back to normal
            
            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Casting = false;// Reset casting boolean
        }

        else // Ability canceled
        {
            // Cursor goes back to normal
            
            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Casting = false; // Reset casting boolean
        }
    }

    public void AutoAttacked()
    {
        AutoAttacksLaunched++;
        AutoAttackEndTimer.WaitTime = 3;
        AutoAttackEndTimer.Start();
        AbilityUI.UpdateLeapCooldown("Ability4");
    }

    public void _on_auto_attack_end_timer_timeout()
    {
        GD.Print(AttackSpeedEffects);

        foreach (StatEffect effect in AttackSpeedEffects)
        {
            effect.Exit();
        }
        AttackSpeedEffects.Clear();
        AutoAttackEndTimer.Stop();
    }

}

