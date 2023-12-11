using Godot;
using System;

public partial class HealthBar3D : Sprite3D
{
	SubViewport Viewport;
    HealthBar2D HealthBar2D;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Viewport = GetNode<SubViewport>("HealthBarViewport");
		HealthBar2D = Viewport.GetNode<HealthBar2D>("HealthBar2D");

        Texture = Viewport.GetTexture();
    }

	public void Update(int value, int full)
	{
        HealthBar2D.UpdateBar(value, full);
    }
}
