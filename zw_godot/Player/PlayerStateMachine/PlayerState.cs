using Godot;
using System;

public partial class PlayerState : SimpleState
{
	public Player Player;
    public SimpleStateMachine StateMachine;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StateMachine = (SimpleStateMachine)GetParent().GetParent();
    	Player = (Player)StateMachine.GetParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
