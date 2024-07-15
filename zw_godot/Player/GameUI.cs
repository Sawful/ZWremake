using Godot;
using System;

public partial class GameUI : Control
{

    public Player Player;
    public int UpgradePoint = 50;
    public Label UpgradePointCounter;

    AbilityUI AbilityUI;

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


    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        UpgradePointCounter = (Label)GetNode("TopRightDisplay").GetNode("UpgradePointCounter");
        UpgradePointCounter.Text = "Upgrade Points: " + UpgradePoint.ToString();

        AbilityUI = (AbilityUI)GetNode("BottomBar").GetNode("AbilityUI");

        Resource = AbilityUI.GetNode<Label>("ResourceText");

        // Set Top Left Display
        TopLeftDisplay = GetNode<BoxContainer>("TopLeftDisplay");

            // Set Rewards Section
            Level = TopLeftDisplay.GetNode<Label>("LevelText");
            Experience = TopLeftDisplay.GetNode<Label>("ExperienceText");
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

	public void UpgradeStats(string stat, int cost)
	{
        Player.StatsLevel[stat] += 1;
        Player.UpdateStats();
        Player.Resource -= cost;
        Resource.Text = "Resource: " + Player.GetResource().ToString();
    }

    public void UpgradeAbility(int abilityIndex)
    {
        if(UpgradePoint > 0 & AbilityUI.AbilityResource[abilityIndex].AbilityLevel < 5)
        {
            UpgradePoint -= 1;
            AbilityUI.AbilityResource[abilityIndex].AbilityLevel += 1;

            GD.Print("Ability number: ");
            GD.Print(abilityIndex);
            GD.Print("Is now level: ");
            GD.Print(AbilityUI.AbilityResource[abilityIndex].AbilityLevel);

            UpgradePointCounter.Text = "Upgrade Points: " + UpgradePoint.ToString();
        }

        
    }

    public bool TestCost(int cost)
    {
        return Player.Resource >= cost;
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
