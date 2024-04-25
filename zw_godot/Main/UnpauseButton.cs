using Godot;
using System;

public partial class UnpauseButton : Button
{
	Node Parent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parent = GetParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	void _on_pressed()
	{
		GetTree().Paused = false;
		Parent.QueueFree();
	}
}
