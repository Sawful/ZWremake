using Godot;
using System;
using System.Text;
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

    public TaskCompletionSource<bool> AbilityCast = new();
    public TaskCompletionSource<bool> AttackMoveContinue = new();


    public override void _Ready()
	{
        AreaIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/AreaIndicator.tscn");
        ArrowIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/ArrowIndicator.tscn");
        ConeIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/ConeIndicator.tscn");
        AreaHitbox = (PackedScene)ResourceLoader.Load("res://Player/Abilities/AreaHitbox.tscn");
        ArrowHitbox = (PackedScene)ResourceLoader.Load("res://Player/Abilities/ArrowHitbox.tscn");

        Player = GetParent<Player>();
        MainCamera = Player.MainCamera;
        Ground = Player.Ground;
        AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");

        Main = Player.GetParent<Node3D>();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if ((Casting | Player.ClosestTarget != null) & AttackMoving) 
        {
            AttackMoveContinue.SetResult(true);
            AttackMoving = false;
        }
            
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

    public async void Overstrike(Entity caster)
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
                        {"Ability", "Overstrike"}
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

            AbilityUI.SetAbilityCooldown("Ability2");
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

            AbilityUI.SetAbilityCooldown("Ability3");
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

            AbilityUI.SetAbilityCooldown("Ability4");
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
}

