using Godot;
using System;

public partial class AreaHitbox : Area3D
{
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    private Camera3D Camera3D;
    private Player Player;
    private Vector3 PointHit;

    public float range;

    // Raycast lenght
    private const float RayLength = 1000.0f;

    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        Camera3D = GetTree().Root.GetNode("Main").GetNode<Node3D>("CameraScript").GetNode<Camera3D>("MainCamera");

        range = 4;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        SetPosition();
    }

    public void SetPosition()
    {
        Vector2 mouse_pos = GetViewport().GetMousePosition();
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = Camera3D.ProjectRayOrigin(mouse_pos),
            To = Camera3D.ProjectRayNormal(mouse_pos) * RayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = MouseColliderLayers,
        };

        var hitDictionary = Player.GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0)
        {
            PointHit = (Vector3)hitDictionary["position"];
        }
        Position = Player.Position + (PointHit - Player.Position).Normalized() * Mathf.Min(PointHit.DistanceTo(Player.Position), range);
    }
}
