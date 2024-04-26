using Godot;
using System;
using System.Collections.Generic;

public partial class TankEnemy : Enemy
{
    public override void _Ready()
    {
        MaxHealth = 80;
        Damage = 5;
        Range = 2;
        Speed = 1.5f;
        AttackSpeed = 0.6f;

        base._Ready();
        EnemyStateMachine = (SimpleStateMachine)GetNode("EnemyStateMachine");

        RessourceOnDeath = 2;
        ExperienceOnDeath = 3;
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