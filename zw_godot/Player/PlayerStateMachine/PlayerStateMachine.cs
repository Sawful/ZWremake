using Godot;
using System;

public partial class PlayerStateMachine : SimpleStateMachine
{
    public Player Player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
        Player = (Player)GetParent();
        ChangeState("IdleState");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
