using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class PointAndClickAbility : Ability
{
	// Called when the node enters the scene tree for the first time.
	    public override void _Ready()
	{
        base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void PointAndClick(Entity caster)
	{
		SetTask();

		if (await Handler.AbilityCast.Task == true)
        {
			Call("Ability", caster);

            EndTask();
        }

        else
        {
			// Cancelled
            EndTask();
        }
    }
}

