using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityUI : ItemList
{
    const int MaxCooldown = 999;

    public Dictionary<string, Button> AbilityButton;

    public CompressedTexture2D abilityImage1;
    public CompressedTexture2D abilityImage2;
    public CompressedTexture2D abilityImage3;
    public CompressedTexture2D abilityImage4;

    public Dictionary<string, Label> AbilityCooldownText;

    public Player Player;

    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");

        AbilityButton = new()
        {
            {"Ability1", GetNode<Button>("AbilityButton1")},
            {"Ability2", GetNode<Button>("AbilityButton2")},
            {"Ability3", GetNode<Button>("AbilityButton3")},
            {"Ability4", GetNode<Button>("AbilityButton4")},
        };

        AbilityCooldownText = new()
        {
            {"Ability1", AbilityButton["Ability1"].GetNode<Label>("AbilityCooldownText1")},
            {"Ability2", AbilityButton["Ability2"].GetNode<Label>("AbilityCooldownText2")},
            {"Ability3", AbilityButton["Ability3"].GetNode<Label>("AbilityCooldownText3")},
            {"Ability4", AbilityButton["Ability4"].GetNode<Label>("AbilityCooldownText4")},
        };

        AbilityCooldownText["Ability1"].Text = "";
        AbilityCooldownText["Ability2"].Text = "";
        AbilityCooldownText["Ability3"].Text = "";
        AbilityCooldownText["Ability4"].Text = "";

        abilityImage1 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability1.png");
        abilityImage2 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability2.png");
        abilityImage3 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability3.png");
        abilityImage4 = (CompressedTexture2D)ResourceLoader.Load("res://Visual/Ui/Icon/ability4.png");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        for (int i = 1; i < 5; i++)
        {
            string currentAbility = "Ability" + i.ToString();

            AbilityCooldown(currentAbility, delta);
        }
    }

    private void AbilityCooldown(string ability, double delta)
    {
        if (Player.IsAbilityCooldown[ability])
        {

            Player.CurrentAbilityCooldown[ability] = Math.Clamp(Player.CurrentAbilityCooldown[ability] - (float)delta, 0, MaxCooldown);
            if (Player.CurrentAbilityCooldown[ability] == 0)
            {
                AbilityButton[ability].Disabled = false;
                Player.IsAbilityCooldown[ability] = false;
                if (AbilityCooldownText[ability] != null)
                {
                    AbilityCooldownText[ability].Text = "";
                }
            }
            else
            {
                if (AbilityCooldownText[ability] != null)
                {
                    AbilityCooldownText[ability].Text = Mathf.Ceil(Player.CurrentAbilityCooldown[ability]).ToString();
                }
            }
        }
    }

    public void SetAbilityCooldown(string ability)
    {
        AbilityButton[ability].Disabled = true;
        Player.IsAbilityCooldown[ability] = true;
        Player.CurrentAbilityCooldown[ability] = Player.AbilityCooldown[ability];
    }

    public void OnAbilityButton1Pressed()
    {
        Player.AbilityScript.Call(Player.Ability["Ability1"], Player);
    }

    public void OnAbilityButton2Pressed()
    {
        Player.AbilityScript.Call(Player.Ability["Ability2"], Player);
    }

    public void OnAbilityButton3Pressed()
    {
        Player.AbilityScript.Call(Player.Ability["Ability3"], Player);
    }

    public void OnAbilityButton4Pressed()
    {
        Player.AbilityScript.Call(Player.Ability["Ability4"], Player);
    }

}
