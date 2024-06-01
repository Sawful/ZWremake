using Godot;
using System;

public partial class UpgradeStatButton : Button
{
	int Cost;
	int StatLevel;
	[Export] public string Stat;
	GameUI gameUI;


	public override void _Ready()
	{
		Cost = 5;
		StatLevel = 0;
		gameUI = GetParent().GetParent().GetParent<GameUI>();
		TooltipText = "Upgrade " + Stat +". Cost: " + Cost.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _on_pressed()
	{
		if(gameUI.TestCost(Cost))
		{
			gameUI.UpgradeStats(Stat, Cost);

			StatLevel += 1;
			Cost = StatLevel ^ 2 + StatLevel * 3 + 5;

			TooltipText = "Upgrade " + Stat +". Cost: " + Cost.ToString();
		}
	}
}
