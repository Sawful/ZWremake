using Godot;
using System;

public partial class AbilityUI : ItemList
{
    const int MaxCooldown = 999;

    public Button AbilityButton1;
    public Button AbilityButton2;
    public Button AbilityButton3;
    public Button AbilityButton4;

    public CompressedTexture2D abilityImage1;
    public CompressedTexture2D abilityImage2;
    public CompressedTexture2D abilityImage3;
    public CompressedTexture2D abilityImage4;

    public Label AbilityCooldownText1;
    public Label AbilityCooldownText2;
    public Label AbilityCooldownText3;
    public Label AbilityCooldownText4;

    public float AbilityCooldown1 = 6;
    public float AbilityCooldown2 = 10;
    public float AbilityCooldown3 = 8;
    public float AbilityCooldown4 = 12;

    public float CurrentAbilityCooldown1;
    public float CurrentAbilityCooldown2;
    public float CurrentAbilityCooldown3;
    public float CurrentAbilityCooldown4;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;
    private bool isAbility4Cooldown = false;

    public override void _Ready()
	{
        AbilityButton1 = GetNode<Button>("AbilityButton1");
        AbilityButton2 = GetNode<Button>("AbilityButton2");
        AbilityButton3 = GetNode<Button>("AbilityButton3");
        AbilityButton4 = GetNode<Button>("AbilityButton4");

        AbilityCooldownText1 = AbilityButton1.GetNode<Label>("AbilityCooldownText1");
        AbilityCooldownText2 = AbilityButton2.GetNode<Label>("AbilityCooldownText2");
        AbilityCooldownText3 = AbilityButton3.GetNode<Label>("AbilityCooldownText3");
        AbilityCooldownText4 = AbilityButton4.GetNode<Label>("AbilityCooldownText4");
        AbilityCooldownText1.Text = "";
        AbilityCooldownText2.Text = "";
        AbilityCooldownText3.Text = "";
        AbilityCooldownText4.Text = "";

        abilityImage1 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability1.png");
        abilityImage2 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability2.png");
        abilityImage3 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability3.png");
        abilityImage4 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability4.png");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        if (AbilityButton1.ButtonPressed == true)
        {
            AbilityButton1.Disabled = true;
            AbilityCooldownText1.Text = "clicked";
            isAbility1Cooldown = true;
            CurrentAbilityCooldown1 = AbilityCooldown1;

        }
        if (AbilityButton2.ButtonPressed == true)
        {
            AbilityButton2.Disabled = true;
            AbilityCooldownText2.Text = "clicked";
            isAbility2Cooldown = true;
            CurrentAbilityCooldown2 = AbilityCooldown2;
        }
        if (AbilityButton3.ButtonPressed == true)
        {
            AbilityButton3.Disabled = true;
            AbilityCooldownText3.Text = "clicked";
            isAbility3Cooldown = true;
            CurrentAbilityCooldown3 = AbilityCooldown3;
        }
        if (AbilityButton4.ButtonPressed == true)
        {
            AbilityButton4.Disabled = true;
            AbilityCooldownText4.Text = "clicked";
            isAbility4Cooldown = true;
            CurrentAbilityCooldown4 = AbilityCooldown4;
        }

        AbilityCooldown(ref CurrentAbilityCooldown1, ref isAbility1Cooldown, AbilityCooldownText1, AbilityButton1, delta);
        AbilityCooldown(ref CurrentAbilityCooldown2, ref isAbility2Cooldown, AbilityCooldownText2, AbilityButton2, delta);
        AbilityCooldown(ref CurrentAbilityCooldown3, ref isAbility3Cooldown, AbilityCooldownText3, AbilityButton3, delta);
        AbilityCooldown(ref CurrentAbilityCooldown4, ref isAbility4Cooldown, AbilityCooldownText4, AbilityButton4, delta);


    }

    private static void AbilityCooldown(ref float currentCooldown, ref bool isCooldown, Label skillText, Button abilityButton, double delta)
    {
        if (isCooldown)
        {
            currentCooldown = Math.Clamp(currentCooldown - (float)delta, 0, MaxCooldown);

            if (currentCooldown <= 0)
            {
                abilityButton.Disabled = false;
                isCooldown = false;
                currentCooldown = 0;
                if (skillText != null)
                {
                    skillText.Text = "";
                }
            }
            else
            {
                if (skillText != null)
                {
                    skillText.Text = Mathf.Ceil(currentCooldown).ToString();
                }
            }
        }
    }
}
