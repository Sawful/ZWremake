using Godot;
using System;
using System.Collections.Generic;

public partial class RangeEnemy : Enemy
{
    public override void _Ready()
    {
        MaxHealth = 15;
        Damage = 3;
        Range = 4;
        Speed = 2;
        AttackSpeed = 1.4f;

        base._Ready();
        EnemyStateMachine = (SimpleStateMachine)GetNode("EnemyStateMachine");

        RessourceOnDeath = 3;
        ExperienceOnDeath = 2;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!IsInstanceValid(Player))
        {
            EnemyStateMachine.ChangeState("IdleState");
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
