using Godot;
using System;
using System.Collections.Generic;

public partial class AttackingState : SimpleState
{
    SimpleStateMachine StateMachine;
    Enemy Target;
    Player Player;
    Dictionary<string, object> Message;
    public override void _Ready()
	{
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Player = (Player)StateMachine.GetParent();
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;
        Target = (Enemy)Message["Target"];
    }

    
    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);
        
        if (!IsInstanceValid(Target))
        {
            StateMachine.ChangeState("IdleState");
        }

        else
        {
            Vector3 targetPosition = Target.Position;

            if (Player.Position.DistanceTo(targetPosition) >= Player.Range)
            {
                Player.MoveTo(dt, targetPosition);
            }

            else
            {
                Player.RotateTo(targetPosition, Entity.RotationWeight);

                if (Player.AttackReload <= 0)
                {
                    Player.DealDamage(Target, Player.Damage);
                    Player.AttackReload = 1 / Player.AttackSpeed;
                }
            }
        }
    }
}
