using Godot;
using System;
using System.Collections.Generic;

public partial class BaseEnemy : Enemy
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        MaxHealth = 30;
        Damage = 3;
        Range = 2;
        Speed = 2;
        AttackSpeed = 1;

        base._Ready();
        EnemyStateMachine = (SimpleStateMachine)GetNode("EnemyStateMachine");

        

        RessourceOnDeath = 1;
        ExperienceOnDeath = 1;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (!IsInstanceValid(Player))
        {
            EnemyStateMachine.ChangeState("EnemyIdleState");
        }

        else
        {
            // Get player position
            PlayerPos = Player.Position;

            Dictionary<string, object> message = new()
                {
                    { "Target", Player }
                };

            if (Range > Position.DistanceTo(PlayerPos))
            {
                EnemyStateMachine.ChangeState("EnemyAttackingState", message);
            }

            else
            {
                EnemyStateMachine.ChangeState("EnemyDirectMovingState", message);
            }
        }
    }
}
