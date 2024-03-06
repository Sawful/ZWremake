using Godot;
using System;

public partial class GameUI : Control
{

    public Player Player;
    public int UpgradePoint;
    public Label UpgradePointCounter;

    private BoxContainer TopLeftDisplay;
    private Label Level;
    private Label Experience;
    private Label Resource;
    private Label Damage;
    private Label AttackSpeed;
    private Label AbilityHaste;
    private Label Speed;
    private Label HealthRegeneration;

    HealthBar HealthBar;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        UpgradePointCounter = (Label)GetNode("TopRightDisplay").GetNode("UpgradePointCounter");
        UpgradePointCounter.Text = "Upgrade Points: " + UpgradePoint.ToString();

        // Set Top Left Display
        TopLeftDisplay = GetNode<BoxContainer>("TopLeftDisplay");

            // Set Rewards Section
            Level = TopLeftDisplay.GetNode<Label>("LevelText");
            Experience = TopLeftDisplay.GetNode<Label>("ExperienceText");
            Resource = TopLeftDisplay.GetNode<Label>("ResourceText");
            GetRewards();

            // Set Stats Section
            Damage = TopLeftDisplay.GetNode<Label>("DamageText");
            AttackSpeed = TopLeftDisplay.GetNode<Label>("AttackSpeedText");
            AbilityHaste = TopLeftDisplay.GetNode<Label>("AbilityHasteText");
            Speed = TopLeftDisplay.GetNode<Label>("SpeedText");
            HealthRegeneration = TopLeftDisplay.GetNode<Label>("HealthRegenerationText");
            UpdateStats();

        // Set HealthBar
        HealthBar = GetNode<HealthBar>("HealthBar");
        HealthBar.SetPlayer(Player);
        
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

    public void GetRewards()
    {
        Level.Text = "Level: " + Player.GetLevel().ToString();
        Experience.Text = "Exp: " + Player.GetExperience().ToString();
        Resource.Text = "Resource: " + Player.GetResource().ToString();
    }

    public void UpdateStats()
    {
        Damage.Text = "Damage: " + Player.GetDamage().ToString();
        AttackSpeed.Text = "Attack Speed: " + Player.GetAttackSpeed().ToString();
        AbilityHaste.Text = "Ability Haste: " + Player.GetAbilityHaste().ToString();
        Speed.Text = "Speed: " + Player.GetSpeed().ToString();
        HealthRegeneration.Text = "Health Regeneration: " + Player.GetHealthRegeneration().ToString();
    }
}
