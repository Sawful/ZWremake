using Godot;
using System;

public partial class Enemy : Node3D
{
	public MoveAndAttack Player;
    private Vector3 PlayerPos;


    // Stats
    [Export] private int Health = 30;
    [Export] private int Damage = 5;
    [Export] private float Range = 2;
    [Export] private float Speed = 2.0f;
    [Export] private double AttackSpeed = 1;
    [Export] private double AttackReload = 0;

    // States
    private bool MoveState;
    private bool AttackState;

    private const float RotationWeight = 0.1f;

    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<MoveAndAttack>("Player");

        // Initialise states
        MoveState = true;
        AttackState = false;
    }

	public override void _PhysicsProcess(double delta)
	{
		// Get player position
        PlayerPos = Player.Position;
        AttackReload -= delta;
        #region States
        SetStatesBasedOnDistance();

        // Move state
        if (MoveState)
        {
            RotateTo(PlayerPos, RotationWeight);
            Move(PlayerPos, delta);
        }

        // Attack state
        else if (AttackState)
        {
            RotateTo(PlayerPos, RotationWeight);
            if (AttackReload <= 0)
            {
                DealDamage(Player);
                AttackReload = 1 / AttackSpeed;
                GD.Print("Damage");
            }
        }
        #endregion
    }


    private void SetStatesBasedOnDistance()
    {
        if (Range > Position.DistanceTo(PlayerPos))
        {
            AttackState = true;
            MoveState = false;
        }
        else
        {
            MoveState = true;
            AttackState = false;
        }
    }

    private void RotateTo(Vector3 rotationTo, float weight)
    {
        var newRotationY = Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(rotationTo.X - Position.X, rotationTo.Z - Position.Z), weight);
        Rotation = new Vector3(Rotation.X, newRotationY, Rotation.Z);
    }

    private void Move(Vector3 movePoint, double delta)
	{
        Position = Position.MoveToward(movePoint, Speed * Convert.ToSingle(delta));
    }

    private void DealDamage(MoveAndAttack target)
    {
        target.TakeDamage(this, Damage);
    }

    public void TakeDamage(MoveAndAttack attacker, int damageAmount)
    {
        Health -= damageAmount;
        GD.Print("damage dealt");
        GD.Print("current target health:" + Health.ToString());
        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if (Health <= 0)
        {
            this.QueueFree();
        }
    }
}
