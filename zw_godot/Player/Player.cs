using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


public partial class Player : Entity
{


    // Nodes
    public Node Main;
    public Camera3D MainCamera;
    [Export] public StaticBody3D Ground;
    private Enemy EnemyClicked;
    public SimpleStateMachine PlayerStateMachine;
    public Node3D ClosestTarget;
    AbilityUI AbilityUI;
    GameUI GameUI;
    Area3D Area3D;
    public Godot.Collections.Array<Node3D> OverlappingBodies;

    public PackedScene SoundEffectPlayer;
    public AudioStreamWav AttackSound;

    // Ranged attack
    public bool RangedAttack;
    public const float ProjectileSpeed = 10;

    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    // Raycast length
    private const float RayLength = 1000.0f;

    public CollisionObject3D[] possibleTargets;

    // Abilities
    public AbilityHandler AbilityScript;
    public Godot.Collections.Array<Node> AbilityArray;
    public Dictionary<string, Node> Ability;
    public Dictionary<string, float> AbilityCooldown;
    public Dictionary<string, float> CurrentAbilityCooldown;
    public Dictionary<string, bool> IsAbilityCooldown;
    public Dictionary<string, string> AbilityPath;

    // Vectors
    private Vector3 AnchorPoint = Vector3.Zero;
    private Vector3 CameraLocalStartingPosition;
    private Vector3 EnemyPos;

    public Dictionary<string, int> StatsLevel;

    public Dictionary<string, double> StatsBonusMult;
    public Dictionary<string, double> StatsBonusAdd;

    Godot.Timer RegenerationTimer;

    public string PlayerClass;

    private int Experience;
    private int Level = 1;
    private int ExperienceToLevelUp;
    private int Resource;
    
	private PlayerInfo PlayerInfo;

    public override void _Ready()
    {
        PlayerInfo = GetNode<PlayerInfo>("/root/PlayerInfo");
        Main = GetTree().Root.GetNode("Main");

        GameUI = GetTree().Root.GetNode("Main").GetNode<GameUI>("PlayerUI");
        AbilityUI = GameUI.GetNode<PanelContainer>("BottomBar").GetNode<AbilityUI>("AbilityUI");

        RegenerationTimer = GetNode<Godot.Timer>("RegenerationTimer");

        AbilityScript = GetNode<AbilityHandler>("Abilities");

        MainCamera = GetTree().Root.GetNode("Main").GetNode<CameraScript>("MainCamera");
        AbilityScript.MainCamera = MainCamera;
        AbilityArray = AbilityScript.GetChildren();

        Ability = new()
        {
            {"Ability1", AbilityArray[0]},
            {"Ability2", AbilityArray[1]},
            {"Ability3", AbilityArray[2]},
            {"Ability4", AbilityArray[3]}
        };

        PlayerClass = PlayerInfo.PlayerClass;
        GD.Print(PlayerClass);

        ExperienceToLevelUp = 5;

        StatsLevel = new()
        {
            {"Damage", 0},
            {"AttackSpeed", 0},
            {"MovementSpeed", 0},
            {"MaxHealth", 0},
            {"AbilityHaste", 0}
        };

        StatsBonusMult = new()
        {
            {"Damage", 0},
            {"AttackSpeed", 0},
            {"MovementSpeed", 0},
            {"MaxHealth", 0},
            {"AbilityHaste", 0}
        };

        StatsBonusAdd = new()
        {
            {"Damage", 0},
            {"AttackSpeed", 0},
            {"MovementSpeed", 0},
            {"MaxHealth", 0},
            {"AbilityHaste", 0}
        };

        if(PlayerClass == "Warrior")
        {
            AbilityPath = new()
            {
                {"Ability1", "res://Player/Abilities/Scenes/Warrior/" + AbilityArray[0].Name + ".tscn"},
                {"Ability2", "res://Player/Abilities/Scenes/Warrior/" + AbilityArray[1].Name + ".tscn"},
                {"Ability3", "res://Player/Abilities/Scenes/Warrior/" + AbilityArray[2].Name + ".tscn"},
                {"Ability4", "res://Player/Abilities/Scenes/Warrior/" + AbilityArray[3].Name + ".tscn"}
            };

            Range = 2;
            RangedAttack = false;
        }

        else if(PlayerClass == "Sorcerer")
        {
            Range = 4;
            RangedAttack = true;
        }

        else if(PlayerClass == "Archer")
        {
            Range = 4;
            RangedAttack = true;
        }

        else
        {
            GD.Print("Unknown Class");
        }

        AbilityCooldown = new()
        {
            {"Ability1", 6},
            {"Ability2", 10},
            {"Ability3", 8},
            {"Ability4", 12}
        };
        CurrentAbilityCooldown = new()
        {
            {"Ability1", 0},
            {"Ability2", 0},
            {"Ability3", 0},
            {"Ability4", 0}
        };
        IsAbilityCooldown = new()
        {
            {"Ability1", false},
            {"Ability2", false},
            {"Ability3", false},
            {"Ability4", false}
        };

        MaxHealth = (int)Math.Round((200 + StatsBonusAdd["MaxHealth"] + 10 * StatsLevel["MaxHealth"]) * (1 + StatsBonusMult["MaxHealth"]));
        Damage = (int)Math.Round((10 + StatsBonusAdd["Damage"] + 3 * StatsLevel["Damage"]) * (1 + StatsBonusMult["Damage"]));
        Speed = (float) (4 + StatsBonusAdd["MovementSpeed"] + 0.5f * StatsLevel["MovementSpeed"]) * (float) (1 + StatsBonusMult["MovementSpeed"]);
        AttackSpeed = (1 + StatsBonusAdd["AttackSpeed"] + 0.2 * StatsLevel["AttackSpeed"]) * (1 + StatsBonusMult["AttackSpeed"]);
        AbilityHaste = (int)Math.Round((0 + StatsBonusAdd["AbilityHaste"] + 3 * StatsLevel["AbilityHaste"]) * (1 + StatsBonusMult["AbilityHaste"]));

        HealthRegeneration = 3;
        RegenerationTimer.WaitTime = 1/HealthRegeneration;

        base._Ready();

        SoundEffectPlayer = (PackedScene)ResourceLoader.Load("res://Sound/SoundEffect.tscn");
        AttackSound = (AudioStreamWav)ResourceLoader.Load("res://Sound/hitsound.wav");

        PlayerStateMachine = (SimpleStateMachine)GetNode("PlayerStateMachine");
        Area3D = GetNode<Area3D>("Area3D");
        // Camera initialisation
        CameraLocalStartingPosition = ToLocal(MainCamera.GlobalPosition);
    }

    public override void _Process(double delta)
    {

        if (Input.IsActionJustPressed("AttackMoveKey"))
        {
            AbilityScript.Call("AttackMove");
        }
    }



    public override void _PhysicsProcess(double delta)
    {
        AttackReload -= delta;

        GetEnemies();

    }

    public void RightClickRaycast(InputEventMouseButton rClick)
    {
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = MainCamera.ProjectRayOrigin(rClick.Position),
            To = MainCamera.ProjectRayOrigin(rClick.Position) + MainCamera.ProjectRayNormal(rClick.Position) * RayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = MouseColliderLayers,
        };

        // Checks if Raycast hit something
        var hitDictionary = GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0) 
        {
            var objectHit = hitDictionary["collider"].Obj;

            AbilityScript.AbilityCast.SetResult(false);
            AbilityScript.AbilityCast = new TaskCompletionSource<bool>();
            AbilityScript.Casting = false;

            if (objectHit == Ground)
            {
                AnchorPoint = (Vector3)hitDictionary["position"];
                Dictionary<string, object> message = new()
                {
                    { "MovePoint", AnchorPoint }
                };

                PlayerStateMachine.ChangeState("MovingState", message);
            }
            
            else if (objectHit is Enemy enemyHit)
            {
                EnemyClicked = enemyHit;
                Dictionary<string, object> message = new()
                {
                    { "Target", EnemyClicked },
                    { "Projectile Speed", ProjectileSpeed }
                };

                if (RangedAttack) 
                {
                    PlayerStateMachine.ChangeState("RangeAttackingState", message);
                }
                    
                else
                    PlayerStateMachine.ChangeState("AttackingState", message);
            }
        }
    }

    public void AutoAttack(Entity target)
    {
        if (AttackReload <= 0)
        {
            DealDamage(target, Damage);
            AttackReload = 1 / AttackSpeed;
            // Call auto attack
            Ability["Ability4"].Call("AutoAttacked");
        }
    }

    public void OnRegenerationTimerTimeout()
    {
        Health = Math.Min(Health + 1, MaxHealth);
    }

    public void GetEnemies()
    {
        OverlappingBodies = Area3D.GetOverlappingBodies();
        ClosestTarget = GetClosest(OverlappingBodies);
    }

    public virtual Node3D GetClosest(Godot.Collections.Array<Node3D> nodeArray)
    {
        Node3D closest = null;
        double closestDistance = 99999;

        foreach (Node3D node in nodeArray)
        {
            double distance = Position.DistanceTo(node.Position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = node;
            }
        }
        return closest;
    }

    public void GetRewards(int ressource, int experience)
    {
        Resource += ressource;
        Experience += experience;

        while (Experience >= ExperienceToLevelUp)
        {
            LevelUp();
        }
        GameUI.GetRewards();
        
    }

    public void UpdateStats()
    {
        MaxHealth = (int)Math.Round((200 + StatsBonusAdd["MaxHealth"] + 10 * StatsLevel["MaxHealth"]) * (1 + StatsBonusMult["MaxHealth"]));
        Damage = (int)Math.Round((10 + StatsBonusAdd["Damage"] + 3 * StatsLevel["Damage"]) * (1 + StatsBonusMult["Damage"]));
        Speed = (float) (4 + StatsBonusAdd["MovementSpeed"] + 0.5f * StatsLevel["MovementSpeed"]) * (float) (1 + StatsBonusMult["MovementSpeed"]);
        AttackSpeed = (1 + StatsBonusAdd["AttackSpeed"] + 0.2 * StatsLevel["AttackSpeed"]) * (1 + StatsBonusMult["AttackSpeed"]);
        AbilityHaste = (int)Math.Round((0 + StatsBonusAdd["AbilityHaste"] + 3 * StatsLevel["AbilityHaste"]) * (1 + StatsBonusMult["AbilityHaste"]));
        HealthRegeneration = 3;
        RegenerationTimer.WaitTime = 1/HealthRegeneration;

        GameUI.UpdateStats();
    }

    public void LevelUp()
    {
        Experience -= ExperienceToLevelUp;
        Level += 1;
        GameUI.UpgradePoint += 1;
        GameUI.UpgradePointCounter.Text = "Upgrade Points: " + GameUI.UpgradePoint.ToString();
    }

    public int GetLevel()
    {
        return Level;
    }

    public int GetExperience()
    {
        return Experience;
    }

    public int GetResource()
    {
        return Resource;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void SetExperience(int experience)
    {
        Experience = experience;
    }
}
