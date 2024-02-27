using Godot;
using System;
using System.Collections.Generic;

public partial class StatEffect : Node
{
	Timer EffectTimer;
	public Dictionary<string, object> Message;
	public Player Player;

	public override void _Ready()
	{
		EffectTimer = GetNode<Timer>("EffectTimer");
		EffectTimer.WaitTime = (float)Message["Duration"];
		EffectTimer.Start();
		// Change the stats
		Player.StatsBonusMult["AttackSpeed"] = 0.5;
		Player.StatsBonusMult["Damage"] = 0.2;
		Player.UpdateStats();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _on_effect_timer_timeout()
	{
		Exit();
	}

	public void Exit()
	{
		// Remove stat changes
		Player.StatsBonusMult["AttackSpeed"] = 0;
		Player.StatsBonusMult["Damage"] = 0;
		Player.UpdateStats();
		// Remove Node
		QueueFree();
	}
}
