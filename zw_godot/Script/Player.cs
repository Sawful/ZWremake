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
    Area3D Area3D;
    public Godot.Collections.Array<Node3D> OverlappingBodies;

    // Ranged attack
    [Export] private bool RangedAttack;
    [Export] private float ProjectileSpeed = 10;

    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    // Raycast lenght
    private const float RayLength = 1000.0f;


    public CollisionObject3D[] possibleTargets;

    // Abilities
    public Ability Abilities;
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

    // Player States
    private bool ReadyAttackMove = false;
    private bool AttackMove = false;

    public override void _Ready()
    {
        AbilityUI = GetTree().Root.GetNode("Main").GetNode<Control>("PlayerUI").GetNode<PanelContainer>("BottomBar").GetNode<AbilityUI>("AbilityUI");
        Abilities = GetNode<Ability>("Abilities");
        Ability1 = "Overstrike";
        Ability2 = "Flamestorm";
        Ability3 = "Arrowshot";
        Ability4 = "Cone";

        // Stats
        MaxHealth = 200;
        Damage = 10;
        Range = 4;
        Speed = 4;
        AttackSpeed = 1;

        base._Ready();
        PlayerStateMachine = (SimpleStateMachine)GetNode("PlayerStateMachine");
        Area3D = GetNode<Area3D>("Area3D");
        // Camera initialisation
        CameraLocalStartingPosition = ToLocal(Camera3D.GlobalPosition);
    }

    public override void _Process(double delta)
    {
        

        if (Input.IsActionJustPressed("x_key"))
        {
            Abilities.Call("AttackMove");
        }

        if (Input.IsMouseButtonPressed((MouseButton)1) & ReadyAttackMove)
        {
            PlayerStateMachine.ChangeState("AttackMoveState");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        AttackReload -= delta;

        GetEnemies();

        if (AttackMove)
        {
            MoveTo(delta, AnchorPoint);
        }
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

            Abilities.AbilityCast.SetResult(false);
            Abilities.AbilityCast = new TaskCompletionSource<bool>();
            Abilities.Casting = false;

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
                    { "Target", EnemyClicked }
                };
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


}
