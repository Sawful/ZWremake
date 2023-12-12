using Godot;
using System;
public partial class Enemy : Entity
{
	public Player Player;
    private Vector3 PlayerPos;
    HealthBar3D HealthBar;

    // States
    private bool MoveState;
    private bool AttackState;

    int RessourceOnDeath = 1;
    int ExperienceOnDeath = 1;

    public override void _Ready()
	{
        // Stats
        base._Ready();

        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        HealthBar = GetNode<HealthBar3D>("HealthBar3D");
        HealthBar.Update(Health, MaxHealth);

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

    public override void TakeDamage(Entity attacker, int damageAmount)
    {
        base.TakeDamage(attacker, damageAmount);
        HealthBar.Update(Health, MaxHealth);
    }

    public override void Die()
    {
        GiveReward();
        base.Die();
    }

    private void GiveReward()
    {
        Player.GetRewards(RessourceOnDeath, ExperienceOnDeath);
    }
}
