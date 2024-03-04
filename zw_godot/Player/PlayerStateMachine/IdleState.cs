using Godot;
using System;

public partial class IdleState : SimpleState
{
    SimpleStateMachine StateMachine;

    Player Player;

    public override void _Ready()
	{
        StateMachine = (SimpleStateMachine)GetParent().GetParent();

        Player = (Player)StateMachine.GetParent();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            Player.RightClickRaycast(eventMouseButton);
        }
    }
}
