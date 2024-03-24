using Godot;
using System;
using System.Collections.Generic;

public partial class MovingState : SimpleState
{
	SimpleStateMachine StateMachine;
    Vector3 MovePoint;
    Player Player;
    PackedScene ClickVFX;


    public override void _Ready()
	{
        ClickVFX = (PackedScene)ResourceLoader.Load("res://Visual/VFX/Click.tscn");
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Player = (Player)StateMachine.GetParent();
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        MovePoint = (Vector3)message["MovePoint"];
        Node3D click = (Node3D)ClickVFX.Instantiate();
        click.Position = MovePoint + (Vector3.Down * 0.2f);
        AddChild(click);
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

        public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            Player.RightClickRaycast(eventMouseButton);
        }
    }
}
