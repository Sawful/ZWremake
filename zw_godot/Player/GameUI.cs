using Godot;
using System;

public partial class GameUI : Control
{

    public Player Player;
    public int UpgradePoint;
    public Label UpgradePointCounter;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        UpgradePointCounter = (Label)GetNode("TopRightDisplay").GetNode("UpgradePointCounter");
        UpgradePointCounter.Text = "Upgrade Points: " + UpgradePoint.ToString();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void UpgradeStats(string stat)
	{
        if (UpgradePoint > 0)
        {
            Player.StatsLevel[stat] += 1;
            Player.UpdateStats();
            UpgradePoint -= 1;
        }
        UpgradePointCounter.Text = "Upgrade Points: " + UpgradePoint.ToString();
    }
}
