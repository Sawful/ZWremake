using Godot;
using System;
using System.Collections.Generic;
//using System.Collections;
//using System.Collections.Generic;


public partial class move_and_attack : Node3D
{
    // Nodes
    [Export] public RayCast3D rayCast3D;
    [Export] public Camera3D camera3D;
    [Export] public StaticBody3D ground;
    [Export] public enemy enemyClicked;

    // Stats
    [Export] public int maxHealth;
    [Export] public int health;
    [Export] public int damage;
    [Export] public float range;
    [Export] public float speed;
    [Export] public double attackSpeed;
    [Export] public double attackReload = 0;

        // Ranged attack
        [Export] public bool rangedAttack;
        [Export] public float projectileSpeed = 10;

    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint mouseColliderLayers;

    // Raycast lenght
    private const float rayLength = 1000.0f;

   // Vectors
    private Vector3 anchorPoint = Vector3.Zero;
    private Vector3 cameraLocalStartingPosition;
    private Vector3 enemyPos;

    // Player States
    private bool moving = false;
    private bool attacking = false;

    // Constants
    private float rotationWeight = 0.1f;

    public override void _Ready()
    {
        // Initialise stats
        maxHealth = 200;
        health = maxHealth;
        damage = 10;
        range = 4f;
        speed = 4;
        attackSpeed = 1;

        // Camera initialisation
        cameraLocalStartingPosition = ToLocal(camera3D.GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        attackReload += -delta;

        // GD.Print("moving: " + moving.ToString() + "    attacking: " + attacking.ToString());

        // If enemy is dead or disappears, stop attacking
        if (attacking && !IsInstanceValid(enemyClicked))
        {
            attacking = false;
        }

        // Basic movement
        if (moving)
        {
            // Player update
            Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(anchorPoint.X - Position.X, anchorPoint.Z - Position.Z), rotationWeight), Rotation.Z);
            Position = Position.MoveToward(anchorPoint, speed * Convert.ToSingle(delta));
            // Destination reached check
            if (Position == anchorPoint)
            {
                moving = false;
            }
        }
        // Attack state
        else if (attacking)
        {
            enemyPos = enemyClicked.Position;

            if (range > Position.DistanceTo(enemyPos))
            {
                Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(enemyPos.X - Position.X, enemyPos.Z - Position.Z), rotationWeight), Rotation.Z);
                if (attackReload <= 0)
                {
                    Damage(enemyClicked);
                    attackReload = 1 / attackSpeed;
                }
            }

            else
            {
                Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(enemyPos.X - Position.X, enemyPos.Z - Position.Z), rotationWeight), Rotation.Z);
                Position = Position.MoveToward(enemyPos, speed * Convert.ToSingle(delta));
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


    public void RayCast(InputEventMouseButton r_click)
    {
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = camera3D.ProjectRayOrigin(r_click.Position),
            To = camera3D.ProjectRayNormal(r_click.Position) * rayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = mouseColliderLayers,
        };

        // Checks if Raycast hit something
        var hitDictionary = GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0) 
        {
            
            var objectHit = hitDictionary["collider"].Obj;
            if (objectHit == ground)
            {
                
                moving = true;
                attacking = false;
                anchorPoint = (Vector3)hitDictionary["position"];
                GlobalPosition.DirectionTo((Vector3)anchorPoint);
            }
            
            else if (objectHit.GetType().ToString() == "enemy")
            {
                moving = false;
                attacking = true;
                anchorPoint = (Vector3)hitDictionary["position"];
                enemyClicked = (enemy)objectHit;
                
            }
        }
    }


    public void Damage(enemy target)
    {
        target.takeDamage(this, damage);
    }

    public void takeDamage(enemy attacker, int damageAmount)
    {
        health -= damageAmount;
        GD.Print("damage taken");
        GD.Print("current health:" + health.ToString());
    }
}
