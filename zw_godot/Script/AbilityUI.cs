using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityUI : ItemList
{
    const int MaxCooldown = 999;

    public Button AbilityButton1;
    public Button AbilityButton2;
    public Button AbilityButton3;
    public Button AbilityButton4;
    public Button[] AbilityButtons;

    public CompressedTexture2D abilityImage1;
    public CompressedTexture2D abilityImage2;
    public CompressedTexture2D abilityImage3;
    public CompressedTexture2D abilityImage4;

    public Label AbilityCooldownText1;
    public Label AbilityCooldownText2;
    public Label AbilityCooldownText3;
    public Label AbilityCooldownText4;

    public Player Player;

    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");

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
        AbilityCooldown(ref Player.CurrentAbilityCooldown1, ref Player.isAbility1Cooldown, AbilityCooldownText1, AbilityButton1, delta);
        AbilityCooldown(ref Player.CurrentAbilityCooldown2, ref Player.isAbility2Cooldown, AbilityCooldownText2, AbilityButton2, delta);
        AbilityCooldown(ref Player.CurrentAbilityCooldown3, ref Player.isAbility3Cooldown, AbilityCooldownText3, AbilityButton3, delta);
        AbilityCooldown(ref Player.CurrentAbilityCooldown4, ref Player.isAbility4Cooldown, AbilityCooldownText4, AbilityButton4, delta);
    }

    public override void _Input(InputEvent @event) 
    {
        if (@event.IsActionPressed("a_key"))
        {
            AbilityButton1.ButtonPressed = true;
        }
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

    public void OnAbilityButton1Pressed()
    {
        Player.Abilities.Call(Player.Ability1, Player);
    }

    public void SetAbility1Cooldown()
    {
        AbilityButton1.Disabled = true;
        Player.isAbility1Cooldown = true;
        Player.CurrentAbilityCooldown1 = Player.AbilityCooldown1;
    }

    public void OnAbilityButton2Pressed()
    {
        Player.Abilities.Call(Player.Ability2, Player);
    }

    public void SetAbility2Cooldown()
    {
        AbilityButton2.Disabled = true;
        Player.isAbility2Cooldown = true;
        Player.CurrentAbilityCooldown2 = Player.AbilityCooldown2;
    }

    public void OnAbilityButton3Pressed()
    {
        Player.Abilities.Call(Player.Ability3, Player);
    }

    public void SetAbility3Cooldown()
    {
        AbilityButton3.Disabled = true;
        Player.isAbility3Cooldown = true;
        Player.CurrentAbilityCooldown3 = Player.AbilityCooldown3;
    }

    public void OnAbilityButton4Pressed()
    {
        Player.Abilities.Call(Player.Ability4, Player);
    }

    public void SetAbility4Cooldown()
    {
        AbilityButton4.Disabled = true;
        Player.isAbility4Cooldown = true;
        Player.CurrentAbilityCooldown4 = Player.AbilityCooldown4;
    }

}
