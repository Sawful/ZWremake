using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityUI : ItemList
{
    const int MaxCooldown = 999;

    public Dictionary<string, Button> AbilityButton;

    List<Tuple<Button, Label, CompressedTexture2D>> AbilitySettings = new();

    public Player Player;
    public List<AbilityResource> AbilityResource;


    int AbilityCooldownNumber = 4;

    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        AbilityResource = Player.AbilityResource;
        if(Player.PlayerClass == "Warrior"){ AbilityCooldownNumber = 3;}

        for(int i = 1; i < 5; i++)
        {
            AbilitySettings.Add(new Tuple<Button, Label, CompressedTexture2D>(GetNode<Button>("AbilityButton" + i.ToString()),
            GetNode<Button>("AbilityButton" + i.ToString()).GetNode<Label>("AbilityCooldownText" + i.ToString()),
            AbilityResource[i - 1].Icon));

            AbilitySettings[i - 1].Item2.Text = "";
            AbilitySettings[i - 1].Item1.Icon = AbilityResource[i - 1].Icon;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        for (int i = 0; i < AbilityCooldownNumber; i++)
        {
            AbilityCooldown(i, delta);
        }
    }

    private void AbilityCooldown(int abilityIndex, double delta)
    {
        if (AbilityResource[abilityIndex].OnCooldown)
        {
            float cooldown = AbilityResource[abilityIndex].CurrentCooldown;
            Label label = AbilitySettings[abilityIndex].Item2;

            cooldown = Math.Clamp(cooldown - (float)delta, 0, MaxCooldown);
            if (cooldown == 0)
            {
                AbilitySettings[abilityIndex].Item1.Disabled = false;
                AbilityResource[abilityIndex].OnCooldown = false;
                if (label != null)
                {
                    label.Text = "";
                }
            }
            else
            {
                if (label != null)
                {
                    label.Text = Mathf.Ceil(cooldown).ToString();
                }
            }

            AbilityResource[abilityIndex].CurrentCooldown = cooldown;
        }
    }

    public void SetAbilityCooldown(int abilityIndex)
    { 
        AbilitySettings[abilityIndex].Item1.Disabled = true;
        AbilityResource[abilityIndex].OnCooldown = true;
        AbilityResource[abilityIndex].CurrentCooldown = AbilityResource[abilityIndex].Cooldown * Player.CooldownReduction;
        AbilitySettings[abilityIndex].Item2.Text = Mathf.Ceil(AbilityResource[abilityIndex].Cooldown).ToString();
    }

    public void UpdateLeapCooldown(int abilityIndex)
    {
        if(AbilityResource[abilityIndex].OnCooldown)
        {
            int Number = AbilitySettings[abilityIndex].Item2.Text.ToInt();

            AbilitySettings[abilityIndex].Item2.Text = Math.Max(Number - 1, 0).ToString();
            if(AbilitySettings[abilityIndex].Item2.Text.ToInt() == 0)
            {
                AbilitySettings[abilityIndex].Item1.Disabled = false;
                AbilityResource[abilityIndex].OnCooldown = false;
                if (AbilitySettings[abilityIndex].Item2 != null)
                {
                    AbilitySettings[abilityIndex].Item2.Text = "";
                }
            }
        }
    }

    public void OnAbilityButton1Pressed()
    {
        AbilityResource[0].AbilityNode.Call("CastAbility", Player);
    }

    public void OnAbilityButton2Pressed()
    {
        AbilityResource[1].AbilityNode.Call("CastAbility", Player);
    }

    public void OnAbilityButton3Pressed()
    {
        AbilityResource[2].AbilityNode.Call("CastAbility", Player);
    }

    public void OnAbilityButton4Pressed()
    {
        AbilityResource[3].AbilityNode.Call("CastAbility", Player);
    }

}
