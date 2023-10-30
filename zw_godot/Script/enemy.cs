using Godot;
using System;

public partial class enemy : Node3D
{
	public move_and_attack player;
    private Vector3 playerPos;


    // Stats
    public int health = 30;
    public int damage = 5;
    public float range = 2;
    public float speed = 2.0f;
    public double attackSpeed = 1;
    private double attackReload = 0;

    // States
    private bool moveState;
    private bool attackState;

	private float rotationWeight = 0.2f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        player = GetTree().Root.GetNode("Main").GetNode<move_and_attack>("Player");

        // Initialise states
        moveState = true;
        attackState = false;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// Get player position
        playerPos = player.Position;
        attackReload += -delta;
        #region States
        // Set State based on distance to player
        if (range > Position.DistanceTo(playerPos)) 
        {
            attackState = true;
            moveState = false;
        }

        else
        {
            moveState = true;
            attackState = false;
        }

        // Move state
        if (moveState)
        {
            Rotate(playerPos);
            Move(playerPos, delta);
        }

        // Attack state
        else if (attackState)
        {
            Rotate(playerPos);
            if (attackReload <= 0)
            {
                Damage(player);
                attackReload = 1 / attackSpeed;
                GD.Print("damage");
            }
        }
        #endregion
    }

	public void Rotate(Vector3 rotationPoint)
	{
        Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(rotationPoint.X - Position.X, rotationPoint.Z - Position.Z), rotationWeight), Rotation.Z);
    }

	public void Move(Vector3 movePoint, double delta)
	{
        Position = Position.MoveToward(movePoint, speed * Convert.ToSingle(delta));
    }

    public void Damage(move_and_attack target)
    {
        target.takeDamage(this, damage);
    }

    public void takeDamage(move_and_attack attacker, int damageAmount)
    {
        health -= damageAmount;
        GD.Print("damage dealt");
        GD.Print("current target health:" + health.ToString());
        CheckIfDead();
    }

    public void CheckIfDead()
    {
        if (health <= 0)
        {
            
            this.QueueFree();
        }
    }
}
