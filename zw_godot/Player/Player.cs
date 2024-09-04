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
    public GameManager Main;
    public Camera3D MainCamera;
    [Export] public StaticBody3D Ground;
    private Enemy EnemyClicked;
    public SimpleStateMachine PlayerStateMachine;
    AbilityUI AbilityUI;
    GameUI GameUI;
    Area3D TargetArea;

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
    private Vector3 CameraLocalStartingPosition;
    private Vector3 EnemyPos;

    public Dictionary<string, int> StatsLevel;

    public Dictionary<string, float> StatsBonusMult;
    public Dictionary<string, float> StatsBonusAdd;

    Godot.Timer RegenerationTimer;
    PackedScene CritSlowMoTimer;

    public string PlayerClass;

    private int Experience;
    private int Level = 1;
    private int ExperienceToLevelUp;
    public int Resource;

    private NavigationMesh NavMesh;
    private Rid NavMeshRID;
    
	private PlayerInfo PlayerInfo;

    public List<AbilityResource> AbilityResource = new();

    public override void _Ready()
    {
        PlayerInfo = GetNode<PlayerInfo>("/root/PlayerInfo");
        Main = GetParent<GameManager>();
        

        GameUI = Main.GetNode<GameUI>("PlayerUI");
        AbilityUI = GameUI.GetNode<PanelContainer>("BottomBar").GetNode<AbilityUI>("AbilityUI");

        RegenerationTimer = GetNode<Godot.Timer>("RegenerationTimer");
        CritSlowMoTimer = ResourceLoader.Load<PackedScene>("res://Player/Tools/CritSlowMoTimer.tscn");

        AbilityScript = GetNode<AbilityHandler>("Abilities");

        MainCamera = Main.GetNode<CameraScript>("MainCamera");
        AbilityScript.MainCamera = MainCamera;

        PlayerClass = PlayerInfo.PlayerClass;

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
            AbilityResource = PlayerInfo.AbilityResource;
            foreach(AbilityResource abilityResource in AbilityResource)
            {
                if(abilityResource != null)
                {
                    abilityResource.SetAbility(AbilityScript);
                }
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

            //Set default as Warrior
            AbilityResource = PlayerInfo.AbilityResource;
            foreach(AbilityResource abilityResource in AbilityResource)
            {
                if(abilityResource != null)
                {
                    abilityResource.SetAbility(AbilityScript);
                }
            }
            Range = 2;
            RangedAttack = false;
        }

        MaxHealth = (int)Mathf.Round((200 + StatsBonusAdd["MaxHealth"] + 10 * StatsLevel["MaxHealth"]) * (1 + StatsBonusMult["MaxHealth"]));
        Damage = (int)Mathf.Round((10 + StatsBonusAdd["Damage"] + 3 * StatsLevel["Damage"]) * (1 + StatsBonusMult["Damage"]));
        Speed =  (4 + StatsBonusAdd["MovementSpeed"] + 0.5f * StatsLevel["MovementSpeed"]) * (1 + StatsBonusMult["MovementSpeed"]);
        AttackSpeed = (1 + StatsBonusAdd["AttackSpeed"] + 0.2f * StatsLevel["AttackSpeed"]) * (1 + StatsBonusMult["AttackSpeed"]);
        AbilityHaste = (int)Mathf.Round((0 + StatsBonusAdd["AbilityHaste"] + 3 * StatsLevel["AbilityHaste"]) * (1 + StatsBonusMult["AbilityHaste"]));

        HealthRegeneration = 3;
        RegenerationTimer.WaitTime = 1/HealthRegeneration;

        base._Ready();

        SoundEffectPlayer = (PackedScene)ResourceLoader.Load("res://Sound/SoundEffect.tscn");
        AttackSound = (AudioStreamWav)ResourceLoader.Load("res://Sound/hitsound.wav");

        PlayerStateMachine = (SimpleStateMachine)GetNode("PlayerStateMachine");
        TargetArea = GetNode<Area3D>("TargetArea");
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

    public Godot.Collections.Dictionary RightClickRaycast(InputEventMouseButton rClick)
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
            AbilityScript.AbilityCast.SetResult(false);
            AbilityScript.AbilityCast = new TaskCompletionSource<bool>();
            AbilityScript.Casting = false;

            return hitDictionary;
        }

        else return null;
    }

    public void AutoAttack(Entity target)
    {
        if (AttackReload <= 0)
        {
            DealDirectDamage(target, Damage);
            AttackReload = 1 / AttackSpeed;
            // Call auto attack
            foreach (AbilityResource currentAbility in AbilityResource)
            {
                if(currentAbility != null)
                {
                    if(!currentAbility.TimedCooldown)
                    {
                        currentAbility.AbilityNode.Call("AutoAttacked");
                    }
                }
            }
        }
    }

    public override void DealDirectDamage(Entity target, int damageAmount)
    {
        Random rnd = new();
        int critChance =  rnd.Next(100);
        int damageDealt;
        if(critChance == 4)
        {
            GD.Print("Critical Hit");
            damageDealt = Mathf.RoundToInt(damageAmount * DamageDealtMultiplier * target.DamageReceivedMultiplier) * 2;

            if(target.Health - damageDealt <= 0)
            {
                CriticalKill();
            }

            DealDamage(target, damageDealt);
        }
        else
        {
            damageDealt = Mathf.RoundToInt(damageAmount * DamageDealtMultiplier);

            DealDamage(target, damageDealt);
        }
    }

    void CriticalKill()
    {
        Main.StartSlowMo();

        Godot.Timer critTimer = (Godot.Timer)CritSlowMoTimer.Instantiate();
        AddChild(critTimer);
        critTimer.Timeout += Main.StopSlowMo;
    }

    public void DisableAllAbilities()
    {
        for(int i = 0; i<4; i++)
        {
            AbilityUI.DisableAbility(i);
        }
    }
    public void EnableAllAbilities()
    {
        for(int i = 0; i<4; i++)
        {
            AbilityUI.EnableAbility(i);
        }
    }

    public void OnRegenerationTimerTimeout()
    {
        Health = Math.Min(Health + 1, MaxHealth);
    }

    public Godot.Collections.Array<Node3D> GetEnemies()
    {
        return TargetArea.GetOverlappingBodies();
    }
    public Enemy GetClosestEnemy()
    {
        return (Enemy)GetClosest(GetEnemies());
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

    public void GetRewards(int resource, int experience, int threat)
    {
        Resource += resource;
        Experience += experience;

        while (Experience >= ExperienceToLevelUp)
        {
            LevelUp();
        }
        GameUI.GetRewards();

        Main.IncreaseThreat(threat);
    }

    public void UpdateStats()
    {
        MaxHealth = (int)Math.Round((200 + StatsBonusAdd["MaxHealth"] + 10 * StatsLevel["MaxHealth"]) * (1 + StatsBonusMult["MaxHealth"]));
        Damage = (int)Math.Round((10 + StatsBonusAdd["Damage"] + 3 * StatsLevel["Damage"]) * (1 + StatsBonusMult["Damage"]));
        Speed = (float) (4 + StatsBonusAdd["MovementSpeed"] + 0.5f * StatsLevel["MovementSpeed"]) * (float) (1 + StatsBonusMult["MovementSpeed"]);
        AttackSpeed = (1 + StatsBonusAdd["AttackSpeed"] + 0.2f * StatsLevel["AttackSpeed"]) * (1 + StatsBonusMult["AttackSpeed"]);
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
