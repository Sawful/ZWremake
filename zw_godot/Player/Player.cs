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
    [Export] public Camera3D Camera3D;
    [Export] public StaticBody3D Ground;
    [Export] private Enemy EnemyClicked;
    [Export] public SimpleStateMachine PlayerStateMachine;
    public Node3D ClosestTarget;
    AbilityUI AbilityUI;
    Control PlayerUI;
    VBoxContainer TopLeftDisplay;
    Label LevelText;
    Label ExperienceText;
    Label RessourceText;
    ProgressBar HealthBar;
    Label HealthBarText;

    Area3D Area3D;
    public Godot.Collections.Array<Node3D> OverlappingBodies;

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
    public string Ability1;
    public string Ability2;
    public string Ability3;
    public string Ability4;

    public float AbilityCooldown1 = 6;
    public float AbilityCooldown2 = 10;
    public float AbilityCooldown3 = 8;
    public float AbilityCooldown4 = 12;

    public float CurrentAbilityCooldown1;
    public float CurrentAbilityCooldown2;
    public float CurrentAbilityCooldown3;
    public float CurrentAbilityCooldown4;

    public bool isAbility1Cooldown = false;
    public bool isAbility2Cooldown = false;
    public bool isAbility3Cooldown = false;
    public bool isAbility4Cooldown = false;

    // Vectors
    private Vector3 AnchorPoint = Vector3.Zero;
    private Vector3 CameraLocalStartingPosition;
    private Vector3 EnemyPos;

    private int Experience;
    private int Level = 1;
    private int ExperienceToLevelUp;
    private int Ressource;

    public override void _Ready()
    {
        RangedAttack = true;

        PlayerUI = GetTree().Root.GetNode("Main").GetNode<Control>("PlayerUI");
        AbilityUI = PlayerUI.GetNode<PanelContainer>("BottomBar").GetNode<AbilityUI>("AbilityUI");
        HealthBar = PlayerUI.GetNode<ProgressBar>("HealthBar");
        HealthBarText = HealthBar.GetNode<Label>("HealthBarText");
        AbilityScript = GetNode<Ability>("Abilities");

        TopLeftDisplay = PlayerUI.GetNode<VBoxContainer>("TopLeftDisplay");
        LevelText = TopLeftDisplay.GetNode<Label>("LevelText");
        ExperienceText = TopLeftDisplay.GetNode<Label>("ExperienceText");
        RessourceText = TopLeftDisplay.GetNode<Label>("RessourceText");

        LevelText.Text = "Level: " + Level.ToString();
        ExperienceText.Text = "Exp: " + Experience.ToString();
        RessourceText.Text = "Ressource: " + Ressource.ToString();
        ExperienceToLevelUp = 5;

        Ability1 = "Overstrike";
        Ability2 = "Flamestorm";
        Ability3 = "Arrowshot";
        Ability4 = "Cone";

        Ability = new()
        {
            {"Ability1", "Overstrike"},
            {"Ability2", "Flamestorm"},
            {"Ability3", "Arrowshot"},
            {"Ability4", "Cone"}
        };

        // Stats
        MaxHealth = 200;
        Damage = 10;
        Range = 4;
        Speed = 4;
        AttackSpeed = 1;

        base._Ready();

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

        if (Input.IsActionJustPressed("x_key"))
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
            Experience -= ExperienceToLevelUp;
            Level += 1;
        }

        LevelText.Text = "Level: " + Level.ToString();
        ExperienceText.Text = "Exp: " + Experience.ToString();
        RessourceText.Text = "Ressource: " + Ressource.ToString();
    }
}
