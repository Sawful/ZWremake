using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


public partial class Player : Entity
{
    public float CooldownReduction;

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

    private NavigationMesh NavMesh;
    private Rid NavMeshRID;
    
	private PlayerInfo PlayerInfo;

    public List<AbilityResource> AbilityResource = new();

    public override void _Ready()
    {

        PlayerInfo = GetNode<PlayerInfo>("/root/PlayerInfo");
        Main = GetTree().Root.GetNode("Main");

        GameUI = Main.GetNode<GameUI>("PlayerUI");
        AbilityUI = GameUI.GetNode<PanelContainer>("BottomBar").GetNode<AbilityUI>("AbilityUI");

        RegenerationTimer = GetNode<Godot.Timer>("RegenerationTimer");

        AbilityScript = GetNode<AbilityHandler>("Abilities");

        MainCamera = Main.GetNode<CameraScript>("MainCamera");
        AbilityScript.MainCamera = MainCamera;

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
            AbilityResource.Add((AbilityResource)ResourceLoader.Load("res://Player/Abilities/Resources/Warrior1.tres"));
            AbilityResource.Add((AbilityResource)ResourceLoader.Load("res://Player/Abilities/Resources/Warrior2.tres"));
            AbilityResource.Add((AbilityResource)ResourceLoader.Load("res://Player/Abilities/Resources/Warrior3.tres"));
            AbilityResource.Add((AbilityResource)ResourceLoader.Load("res://Player/Abilities/Resources/Warrior4.tres"));
            for(int i = 0; i < 4; i++)
            {
                AbilityResource[i].SetAbility(AbilityScript);
            }
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

        NavMesh = Main.GetNode<NavigationRegion3D>("NavigationRegion3D").NavigationMesh;
        CooldownReduction = 1 - (100 - (10000 / (100 + 2 * AbilityHaste))) / 100;
    }

    public override void _Process(double delta)
    {

        if (Input.IsActionJustPressed("AttackMoveKey"))
        {
            AbilityScript.Call("AttackMove");
        }
    }

    public void _on_nav_agent_target_reached()
    {
        PlayerStateMachine.ChangeState("IdleState");
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
                AnchorPoint = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps()[0], (Vector3)hitDictionary["position"]);
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
            AbilityResource[3].AbilityNode.Call("AutoAttacked");
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
