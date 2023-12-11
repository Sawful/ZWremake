using Godot;
using System;
using System.Collections.Generic;

public partial class MovingState : SimpleState
{
	SimpleStateMachine StateMachine;
    Vector3 MovePoint;
    Player Player;

	public override void _Ready()
	{
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Player = (Player)StateMachine.GetParent();
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        MovePoint = (Vector3)message["MovePoint"];
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);
        Player.MoveTo(dt, MovePoint);
        if (Player.Position == MovePoint)
        {
            StateMachine.ChangeState("IdleState");
        }
    }
}
