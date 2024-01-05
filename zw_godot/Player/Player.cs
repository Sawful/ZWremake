using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
//using System.Collections;
//using System.Collections.Generic;


public partial class Player : Entity
{


    // Nodes
    public Node Main;
    [Export] public Camera3D Camera3D;
    [Export] public StaticBody3D Ground;
    [Export] private Enemy EnemyClicked;
    [Export] public SimpleStateMachine PlayerStateMachine;
    public Node3D ClosestTarget;

    AbilityUI AbilityUI;
    GameUI GameUI;

    VBoxContainer TopLeftDisplay;
    Label LevelText;
    Label ExperienceText;
    Label RessourceText;
    Label DamageText;
    Label AttackSpeedText;
    Label AbilityHasteText;

    ProgressBar HealthBar;
    Label HealthBarText;

    Area3D Area3D;
    public Godot.Collections.Array<Node3D> OverlappingBodies;

    public PackedScene SoundEffectPlayer;
    public AudioStreamWav AttackSound;

    // Ranged attack
    [Export] public bool RangedAttack;
    [Export] public float ProjectileSpeed = 10;

    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    // Raycast lenght
    private const float RayLength = 1000.0f;

    public CollisionObject3D[] possibleTargets;

    // Abilities
    public Ability AbilityScript;

    public Dictionary<string, string> Ability;
    public Dictionary<string, float> AbilityCooldown;
    public Dictionary<string, float> CurrentAbilityCooldown;
    public Dictionary<string, bool> IsAbilityCooldown;

    // Vectors
    private Vector3 AnchorPoint = Vector3.Zero;
    private Vector3 CameraLocalStartingPosition;
    private Vector3 EnemyPos;

    public Dictionary<string, int> StatsLevel;

    private int Experience;
    private int Level = 1;
    private int ExperienceToLevelUp;
    private int Ressource;

    public override void _Ready()
    {
        RangedAttack = true;
        Main = GetTree().Root.GetNode("Main");

        GameUI = GetTree().Root.GetNode("Main").GetNode<GameUI>("PlayerUI");
        AbilityUI = GameUI.GetNode<PanelContainer>("BottomBar").GetNode<AbilityUI>("AbilityUI");
        HealthBar = GameUI.GetNode<ProgressBar>("HealthBar");
        HealthBarText = HealthBar.GetNode<Label>("HealthBarText");
        AbilityScript = GetNode<Ability>("Abilities");

        TopLeftDisplay = GameUI.GetNode<VBoxContainer>("TopLeftDisplay");
        LevelText = TopLeftDisplay.GetNode<Label>("LevelText");
        ExperienceText = TopLeftDisplay.GetNode<Label>("ExperienceText");
        RessourceText = TopLeftDisplay.GetNode<Label>("RessourceText");
        DamageText = TopLeftDisplay.GetNode<Label>("DamageText");
        AttackSpeedText = TopLeftDisplay.GetNode<Label>("AttackSpeedText");
        AbilityHasteText = TopLeftDisplay.GetNode<Label>("AbilityHasteText");

        LevelText.Text = "Level: " + Level.ToString();
        ExperienceText.Text = "Exp: " + Experience.ToString();
        RessourceText.Text = "Ressource: " + Ressource.ToString();
        ExperienceToLevelUp = 5;

        StatsLevel = new()
        {
            {"Damage", 0},
            {"AttackSpeed", 0},
            {"MovementSpeed", 0},
            {"MaxHealth", 0},
            {"AbilityHaste", 0}
        };

        Ability = new()
        {
            {"Ability1", "Overstrike"},
            {"Ability2", "Flamestorm"},
            {"Ability3", "Arrowshot"},
            {"Ability4", "Cone"}
        };
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

        Range = 4;
        UpdateStats();

        base._Ready();

        SoundEffectPlayer = (PackedScene)ResourceLoader.Load("res://Sound/SoundEffect.tscn");
        AttackSound = (AudioStreamWav)ResourceLoader.Load("res://Sound/hitsound.wav");

        HealthBar.MaxValue = MaxHealth;
        HealthBar.Value = Health;

        PlayerStateMachine = (SimpleStateMachine)GetNode("PlayerStateMachine");
        Area3D = GetNode<Area3D>("Area3D");
        // Camera initialisation
        CameraLocalStartingPosition = ToLocal(Camera3D.GlobalPosition);
    }

    public override void _Process(double delta)
    {
        HealthBar.MaxValue = MaxHealth;
        HealthBar.Value = Mathf.Lerp(HealthBar.Value, Health, 0.25);
        HealthBarText.Text = Health.ToString() + " / " + MaxHealth.ToString();

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

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            RightClickRaycast(eventMouseButton);
        }
    }

    public void RightClickRaycast(InputEventMouseButton rClick)
    {
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = Camera3D.ProjectRayOrigin(rClick.Position),
            To = Camera3D.ProjectRayNormal(rClick.Position) * RayLength,
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
                    GD.Print("Range attack works");
                    PlayerStateMachine.ChangeState("RangeAttackingState", message);
                }
                    
                else
                    PlayerStateMachine.ChangeState("AttackingState", message);
            }
        }
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
        Ressource += ressource;
        Experience += experience;

        while (Experience >= ExperienceToLevelUp)
        {
            LevelUp();
        }

        LevelText.Text = "Level: " + Level.ToString();
        ExperienceText.Text = "Exp: " + Experience.ToString();
        RessourceText.Text = "Ressource: " + Ressource.ToString();
    }

    public void UpdateStats()
    {
        MaxHealth = 200 + 10 * StatsLevel["MaxHealth"];
        Damage = 10 + 3 * StatsLevel["Damage"];
        Speed = 4 + 0.5f * StatsLevel["MovementSpeed"];
        AttackSpeed = 1 + 0.2 * StatsLevel["AttackSpeed"];
        AbilityHaste = 0 + 3 * StatsLevel["AbilityHaste"];

        DamageText.Text = "Damage: " + Damage.ToString();
        AttackSpeedText.Text = "Attack Speed: " + AttackSpeed.ToString();
        AbilityHasteText.Text = "Ability Haste: " + AbilityHaste.ToString();
    }

    public void LevelUp()
    {
        Experience -= ExperienceToLevelUp;
        Level += 1;
        GameUI.UpgradePoint += 1;
        GameUI.UpgradePointCounter.Text = "Upgrade Points: " + GameUI.UpgradePoint.ToString();
    }
}
