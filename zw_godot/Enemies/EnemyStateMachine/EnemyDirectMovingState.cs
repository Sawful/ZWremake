using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyDirectMovingState : SimpleState
{
    SimpleStateMachine StateMachine;
    Entity Target;
    Enemy Self;
    Dictionary<string, object> Message;

    public override void _Ready()
    {
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Self = (Enemy)StateMachine.GetParent();
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;
        Target = (Entity)Message["Target"];
    }

    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);

        Self.MoveTo(dt, Target.Position);

    }
}
