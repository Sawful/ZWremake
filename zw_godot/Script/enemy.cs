using Godot;
using System;

public partial class enemy : Node3D
{
	public move_and_attack player;
	private RigidBody3D body;
	private float speed = 2.0f;
	private float rotationWeight = 0.2f;
	private Vector3 playerPos;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		player = GetTree().Root.GetNode("Main").GetNode<move_and_attack>("Player");
        body = GetParent<RigidBody3D>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// Get player position
        playerPos = player.Position;

		// Move state
		Rotate(playerPos);
		Move(playerPos, delta);
    }

	public void Rotate(Vector3 rotationPoint)
	{
        body.Rotation = new Vector3(body.Rotation.X, Mathf.LerpAngle(body.Rotation.Y, Mathf.Atan2(rotationPoint.X - body.Position.X, rotationPoint.Z - body.Position.Z), rotationWeight), body.Rotation.Z);
    }

	public void Move(Vector3 movePoint, double delta)
	{
        body.Position = body.Position.MoveToward(movePoint, speed * Convert.ToSingle(delta));
    }
}
