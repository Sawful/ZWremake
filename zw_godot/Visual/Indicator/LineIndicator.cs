using Godot;
using System;

public partial class LineIndicator : Node3D
{
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;

    private Camera3D Camera3D;
    private Player Player;
    private Vector3 PointHit;
    private Vector3 SupposedPosition;

    // Raycast lenght
    private const float RayLength = 1000.0f;

    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        Camera3D = GetTree().Root.GetNode("Main").GetNode<Camera3D>("MainCamera");
        SetPosition();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        SetPosition();
    }

    public void SetPosition()
    {
        Vector2 mouse_pos = GetViewport().GetMousePosition();
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = Camera3D.ProjectRayOrigin(mouse_pos),
            To = Camera3D.ProjectRayOrigin(mouse_pos) + Camera3D.ProjectRayNormal(mouse_pos) * RayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = MouseColliderLayers,
        };

        var hitDictionary = Player.GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0)
        {
            PointHit = (Vector3)hitDictionary["position"];
        }

        var newRotationY = Mathf.Atan2(PointHit.X - Position.X, PointHit.Z - Position.Z);
        Rotation = new Vector3(Rotation.X, newRotationY, Rotation.Z);
        Position = Player.Position;
    }
}
