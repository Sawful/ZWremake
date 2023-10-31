using Godot;
using System;
using System.Collections.Generic;
//using System.Collections;
//using System.Collections.Generic;


public partial class MoveAndAttack : Node3D
{
    // Nodes
    [Export] private RayCast3D RayCast3D;
    [Export] private Camera3D Camera3D;
    [Export] private StaticBody3D Ground;
    [Export] private Enemy EnemyClicked;

    // Stats
    [Export] private int MaxHealth = 200;
    [Export] private int Health;
    [Export] private int Damage = 10;
    [Export] private float Range = 4;
    [Export] private float Speed = 4;
    [Export] private double AttackSpeed = 1;
    [Export] private double AttackReload = 0;

        // Ranged attack
        [Export] private bool RangedAttack;
        [Export] private float ProjectileSpeed = 10;

    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    // Raycast lenght
    private const float RayLength = 1000.0f;

   // Vectors
    private Vector3 AnchorPoint = Vector3.Zero;
    private Vector3 CameraLocalStartingPosition;
    private Vector3 EnemyPos;

    // Player States
    private bool Moving = false;
    private bool Attacking = false;

    // Constants
    private float RotationWeight = 0.1f;

    public override void _Ready()
    {
        // Initialise stats
        Health = MaxHealth;

        // Camera initialisation
        CameraLocalStartingPosition = ToLocal(Camera3D.GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        AttackReload -= delta;

        // If enemy is dead or disappears, stop attacking
        if (Attacking && !IsInstanceValid(EnemyClicked))
        {
            Attacking = false;
        }

        // Basic movement
        if (Moving)
        {
            MoveTo(delta, AnchorPoint);
        }
        // Attack state
        else if (Attacking)
        {
            EnemyPos = EnemyClicked.Position;

            if (Range > Position.DistanceTo(EnemyPos))
            {
                RotateTo(EnemyPos, RotationWeight);
                if (AttackReload <= 0)
                {
                    DealDamage(EnemyClicked);
                    AttackReload = 1 / AttackSpeed;
                }
            }

            else
            {
                MoveTo(delta, EnemyPos);
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            RayCast(eventMouseButton);
        }
    }


    public void RayCast(InputEventMouseButton rClick)
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
            if (objectHit == Ground)
            {
                Moving = true;
                Attacking = false;
                AnchorPoint = (Vector3)hitDictionary["position"];
                GlobalPosition.DirectionTo(AnchorPoint);
            }
            
            else if (objectHit is Enemy enemyHit)
            {
                Moving = false;
                Attacking = true;
                AnchorPoint = (Vector3)hitDictionary["position"];
                EnemyClicked = enemyHit;
                
            }
        }
    }


    public void MoveTo(double delta, Vector3 moveTo)
    {
        // Player update
        RotateTo(moveTo, RotationWeight);
        Position = Position.MoveToward(moveTo, Speed * Convert.ToSingle(delta));
        // Destination reached check
        if (Position == AnchorPoint)
        {
            Moving = false;
        }
    }

    public void RotateTo(Vector3 rotationPoint, float rotationWeight)
    {
        var newRotationY = Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(rotationPoint.X - Position.X, rotationPoint.Z - Position.Z), rotationWeight);
        Rotation = new Vector3(Rotation.X, newRotationY, Rotation.Z);
    }

    public void DealDamage(Enemy target)
    {
        target.TakeDamage(this, Damage);
    }

    public void TakeDamage(Enemy attacker, int damageAmount)
    {
        Health -= damageAmount;
        GD.Print("damage taken");
        GD.Print("current health:" + Health.ToString());
    }
}
