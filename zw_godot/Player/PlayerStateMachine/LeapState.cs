using Godot;
using System;
using System.Collections.Generic;

public partial class LeapState : SimpleState
{

	Enemy Target;
    Player Player;
	Dictionary<string, object> Message;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
