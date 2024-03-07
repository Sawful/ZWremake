using Godot;
using System;

public partial class AreaAbility : Ability
{
    public PackedScene AreaHitbox;
	public PackedScene AreaIndicator;

	public AreaHitbox CurrentHitbox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		AreaHitbox = (PackedScene)ResourceLoader.Load("res://Player/Abilities/AreaHitbox.tscn");
		AreaIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/AreaIndicator.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void Area(float radius, Entity caster)
	{
        SetTask();
		
        AreaIndicator AreaIndic = (AreaIndicator)AreaIndicator.Instantiate();
        AreaIndic.Scale = 2 * radius * Vector3.One;
        Main.AddChild(AreaIndic);

        AreaHitbox AreaHB = (AreaHitbox)AreaHitbox.Instantiate();
        AreaHB.Scale = 2 * radius * Vector3.One;
        Main.AddChild(AreaHB);
		CurrentHitbox = AreaHB;

		if (await Handler.AbilityCast.Task == true)
        {
            EndTask();
            AreaIndic.QueueFree();
            // Cursor goes back to normal

            Call("Ability", caster);
        }
        else
        {
            AreaIndic.QueueFree();
            // Cursor goes back to normal
            EndTask();
        }
	}
}
