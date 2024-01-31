using Godot;
using System;

public partial class AreaIndicator : Node3D
{
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    private Camera3D Camera3D;
    private Player Player;
    private Vector3 PointHit;
    private Vector3 SupposedPosition;

    public float range;
    public double radius;

    // Raycast lenght
    private const float RayLength = 1000.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        Camera3D = GetTree().Root.GetNode("Main").GetNode<Node3D>("CameraScript").GetNode<Camera3D>("MainCamera");

        range = 4;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        SupposedPosition = AbilityIndicatorRaycast();
        Position = Player.Position + (SupposedPosition - Player.Position).Normalized() * Mathf.Min(SupposedPosition.DistanceTo(Player.Position), range);
    }


    public virtual Vector3 AbilityIndicatorRaycast()
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
        return PointHit;
    }
}
