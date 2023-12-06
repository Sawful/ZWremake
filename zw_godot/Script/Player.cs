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
    AbilityUI AbilityUI;

    // Ranged attack
    [Export] private bool RangedAttack;
    [Export] private float ProjectileSpeed = 10;

    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    // Raycast lenght
    private const float RayLength = 1000.0f;

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
    private bool Moving = false;
    private bool Attacking = false;

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

        // Overstrike = (Overstrike)LoadAbility("Overstrike");


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
            if (Position == AnchorPoint)
            {
                Moving = false;
            }
        }
        // Attack state
        else if (Attacking)
        {

            if (WalkUpAttack(Range, EnemyClicked, delta, Damage, AttackReload))
            {
                AttackReload = 1 / AttackSpeed;
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

            Abilities.AbilityCast.SetResult(false);
            Abilities.AbilityCast = new TaskCompletionSource<bool>();
            Abilities.Casting = false;

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

    public virtual bool WalkUpAttack(float range, Enemy target, double delta, int damage, double attackreload)
    {
        if (Position.DistanceTo(target.Position) >= range)
        {
            MoveTo(delta, target.Position);
            return false;
        }

        else 
        {
            RotateTo(EnemyPos, RotationWeight);

            if (attackreload <= 0)
            {
                DealDamage(target, damage);
                return true;
            }

            return false;
        }
    }
}
