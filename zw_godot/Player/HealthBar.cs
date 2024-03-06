using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	Label HealthLabel;
	Player Player;
	public override void _Ready()
	{
		Player = GetParent<GameUI>().Player;
		HealthLabel = GetNode<Label>("HealthBarText");
	}

	public override void _Process(double delta)
	{
		MaxValue = Player.GetMaxHealth();
        Value = Mathf.Lerp(Value, Player.GetHealth(), 0.25);
        HealthLabel.Text = Player.GetHealth().ToString() + " / " + Player.GetMaxHealth().ToString();
	}

	public void SetPlayer(Player player)
	{
		Player = player;
	}
}
