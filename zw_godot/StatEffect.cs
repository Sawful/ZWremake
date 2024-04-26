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
		SetTimer();
		// Change the stats
		AddEffect();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _on_effect_timer_timeout()
	{
		Exit();
	}

	private void SetTimer()
	{
		if((float)Message["Duration"] != 0)
		{
			EffectTimer = GetNode<Timer>("EffectTimer");
			EffectTimer.WaitTime = (float)Message["Duration"];
			EffectTimer.Start();
		}
	}

	public void Exit()
	{
		// Remove stat changes
		RemoveEffect();
		// Remove Node
		QueueFree();
	}

	private void AddEffect()
	{
		if((string)Message["StatEffect"] == "DamageReceived")
		{
			Player.DamageReceivedMultiplier += (float)Message["EffectAmount"];
		}

		else if((string)Message["StatEffect"] == "DamageDealt")
		{
			Player.DamageDealtMultiplier += (float)Message["EffectAmount"];
		}

		else
		{
			Player.StatsBonusMult[(string)Message["StatEffect"]] += (float)Message["EffectAmount"];
		}

		Player.UpdateStats();
	}

		private void RemoveEffect()
	{
		if((string)Message["StatEffect"] == "DamageReceived")
		{
			Player.DamageReceivedMultiplier -= (float)Message["EffectAmount"];
		}

		else if((string)Message["StatEffect"] == "DamageDealt")
		{
			Player.DamageDealtMultiplier -= (float)Message["EffectAmount"];
		}

		else
		{
			Player.StatsBonusMult[(string)Message["StatEffect"]] -= (float)Message["EffectAmount"];
		}
		Player.UpdateStats();
	}
}
