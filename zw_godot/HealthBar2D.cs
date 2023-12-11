using Godot;
using System;

public partial class HealthBar2D : TextureProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void UpdateBar(int amount, int full)
	{
		MaxValue = full;
		Value = amount;
		GD.Print(amount, full);
    }
}
