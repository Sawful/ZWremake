using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class LineAbility : Ability
{
	public PackedScene ArrowIndicator;
	public PackedScene ArrowHitbox;
	public PackedScene ConeHitbox;
	public PackedScene ConeIndicator;

	public ArrowHitbox CurrentHitbox;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		ConeHitbox = (PackedScene)ResourceLoader.Load("res://Player/Abilities/ConeHitbox.tscn");
		ArrowHitbox = (PackedScene)ResourceLoader.Load("res://Player/Abilities/ArrowHitbox.tscn");
		ArrowIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/ArrowIndicator.tscn");
		ConeIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/ConeIndicator.tscn");
	}

	public async void Line(float length, float width, Entity caster)
	{
        SetTask();
		
        LineIndicator LineIndic = (LineIndicator)ArrowIndicator.Instantiate();
        LineIndic.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(LineIndic);

        ArrowHitbox LineHB = (ArrowHitbox)ArrowHitbox.Instantiate();
        LineHB.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(LineHB);
		CurrentHitbox = LineHB;

		if (await Handler.AbilityCast.Task == true)
        {
            EndTask();
            LineIndic.QueueFree();
            // Cursor goes back to normal

            Call("Ability", caster);
        }
        else
        {
            LineIndic.QueueFree();
            // Cursor goes back to normal
            EndTask();
        }
	}

	public async void Cone(float length, float width, Entity caster)
	{
        SetTask();
		
        LineIndicator ConeIndic = (LineIndicator)ConeIndicator.Instantiate();
        ConeIndic.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ConeIndic);

        ArrowHitbox ConeHB = (ArrowHitbox)ConeHitbox.Instantiate();
        ConeHB.Scale = Vector3.Right * width + Vector3.Back * length + Vector3.Up;
        Main.AddChild(ConeHB);
		CurrentHitbox = ConeHB;

		if (await Handler.AbilityCast.Task == true)
        {
            EndTask();
            ConeIndic.QueueFree();
            // Cursor goes back to normal

            Call("Ability", caster);
        }
        else
        {
            ConeIndic.QueueFree();
            // Cursor goes back to normal
            EndTask();
        }
	}

}
