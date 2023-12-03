using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
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
            EnemyPos = EnemyClicked.Position;

            if (Range > Position.DistanceTo(EnemyPos))
            {
                RotateTo(EnemyPos, RotationWeight);
                if (AttackReload <= 0)
                {
                    DealDamage(EnemyClicked, Damage);
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

        if (@event.IsActionPressed("a_key") & !isAbility1Cooldown)
        {
            AbilityUI.AbilityButton1.ButtonPressed = true;
            Abilities.Call(Ability1, this);
        }

        if (@event.IsActionPressed("z_key") & !isAbility1Cooldown)
        {
            AbilityUI.AbilityButton2.ButtonPressed = true;
            Abilities.Call(Ability2, this);
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

    public void UseAbility(Ability ability)
    {

    }
}
