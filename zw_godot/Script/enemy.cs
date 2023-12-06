using Godot;
using System;
public partial class Enemy : Entity
{
	public Player Player;
    private Vector3 PlayerPos;

    // States
    private bool MoveState;
    private bool AttackState;

    public override void _Ready()
	{
        // Stats
        base._Ready();

        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");

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
        SetStates();

        // Move state
        if (MoveState)
        {
            MoveTo(delta, PlayerPos);
        }

        // Attack state
        else if (AttackState)
        {
            RotateTo(PlayerPos, RotationWeight);
            if (AttackReload <= 0)
            {
                DealDamage(Player, Damage);
                AttackReload = 1 / AttackSpeed;
                GD.Print("Damage");
            }
        }
        #endregion
    }


    private void SetStates()
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
}
