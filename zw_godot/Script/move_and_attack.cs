using Godot;
using System;
//using System.Collections;
//using System.Collections.Generic;


public partial class move_and_attack : Node3D
{
    // Nodes
    [Export] public RayCast3D rayCast3D;
    [Export] public Camera3D camera3D;
    [Export] public StaticBody3D ground;

    // Stats
    [Export] public int speed = 3;

    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint mouseColliderLayers;

    // Raycast lenght
    private const float rayLength = 1000.0f;

   // Vectors
    private Vector3 anchorPoint = Vector3.Zero;
    private Vector3 cameraLocalStartingPosition;

    // Player States
    private bool moving = false;

    // Constants
    private float rotationWeight = 0.1f;

    public override void _Ready()
    {
        cameraLocalStartingPosition = ToLocal(camera3D.GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
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
        PhysicsRayQueryParameters3D query = new()
        {
            From = camera3D.ProjectRayOrigin(r_click.Position),
            To = camera3D.ProjectRayNormal(r_click.Position) * rayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = mouseColliderLayers,
        };


        var hitDictionary = GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0) 
        {
            var objectHit = hitDictionary["collider"].Obj;
            if (objectHit == ground)
            {
                moving = true;
                anchorPoint = (Vector3)hitDictionary["position"];
                GlobalPosition.DirectionTo((Vector3)anchorPoint);
            }
        }
    }
}
