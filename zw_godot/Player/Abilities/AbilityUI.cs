using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityUI : ItemList
{
    const int MaxCooldown = 999;

    public Dictionary<string, Button> AbilityButton;

    List<Tuple<Button, Label, CompressedTexture2D>> AbilitySettings = new() {new Tuple<Button, Label, CompressedTexture2D>(null, null, null),
    new Tuple<Button, Label, CompressedTexture2D>(null, null, null),
    new Tuple<Button, Label, CompressedTexture2D>(null, null, null),
    new Tuple<Button, Label, CompressedTexture2D>(null, null, null),};

    public Player Player;
    public List<AbilityResource> AbilityResource;

    private bool AbilitiesForceDisabled = false;

    List<bool> TimedCooldownAbilities = new();

    public override void _Ready()
	{
        Player = GetTree().Root.GetNode("Main").GetNode<Player>("Player");
        AbilityResource = Player.AbilityResource;

        foreach(AbilityResource abilityResource in AbilityResource)
        {
            if(abilityResource != null)
            {
                int slot = abilityResource.Slot;
                Button abilityButton = GetNode<Button>("AbilityButton" + slot.ToString());
                abilityButton.Disabled = false;
                AbilitySettings[slot - 1] = new Tuple<Button, Label, CompressedTexture2D>(abilityButton,
                abilityButton.GetNode<Label>("AbilityCooldownText" + slot.ToString()),
                abilityResource.Icon);

                AbilitySettings[slot - 1].Item2.Text = "";
                AbilitySettings[slot - 1].Item1.Icon = AbilityResource[slot - 1].Icon;
                if(abilityResource.TimedCooldown){TimedCooldownAbilities.Add(true);}
                else{TimedCooldownAbilities.Add(false);}
            }
        }
    }


    public override void _Process(double delta)
	{
        foreach(AbilityResource abilityResource in AbilityResource)
        {
            if(abilityResource != null)
            {
                if (abilityResource.TimedCooldown)
                {AbilityCooldown(abilityResource.Slot - 1);}
            }
        }
    }

    private void AbilityCooldown(int abilityIndex)
    {
        AbilityResource[abilityIndex].UpdateCooldown();
        if (AbilityResource[abilityIndex].CooldownRecharge)
        {
            SetCooldownLabel(abilityIndex);
        }
    }

    private void SetCooldownLabel(int abilityIndex)
    {
        Label label = AbilitySettings[abilityIndex].Item2;
        label.Text = Mathf.Ceil(AbilityResource[abilityIndex].CurrentCooldown).ToString();
    }

    public void UnlockCooldown(int abilityIndex)
    {
        if(!AbilitiesForceDisabled)
        {
            AbilitySettings[abilityIndex].Item1.Disabled = false;
        }
    }

    public void ClearCooldown(int abilityIndex)
    {
        AbilitySettings[abilityIndex].Item2.Text = "";
        UnlockCooldown(abilityIndex);
    }

    public void SetAbilityCooldown(int abilityIndex)
    { 
        if(AbilityResource[abilityIndex].MultipleCharges)
        {
            AbilityResource[abilityIndex].CooldownRecharge = true;
            AbilityResource[abilityIndex].SetCooldown();

            if(AbilityResource[abilityIndex].OnCooldown)
            {
                AbilitySettings[abilityIndex].Item1.Disabled = true;
            }
        }

        else
        {
            AbilitySettings[abilityIndex].Item1.Disabled = true;
            AbilitySettings[abilityIndex].Item2.Text = AbilityResource[abilityIndex].Cooldown.ToString();
            AbilityResource[abilityIndex].OnCooldown = true;
            AbilityResource[abilityIndex].SetCooldown();
        }
        
    }

    public void SetLeapCooldown(int abilityIndex)
    {
        AbilitySettings[abilityIndex].Item1.Disabled = true;
        AbilityResource[abilityIndex].CurrentCooldown = AbilityResource[abilityIndex].Cooldown;
        AbilitySettings[abilityIndex].Item2.Text = AbilityResource[abilityIndex].Cooldown.ToString();
        AbilityResource[abilityIndex].OnCooldown = true;
    }

    public void UpdateLeapCooldown(int abilityIndex)
    {
        if(AbilityResource[abilityIndex].OnCooldown)
        {
            AbilityResource[abilityIndex].CurrentCooldown = Math.Max(AbilityResource[abilityIndex].CurrentCooldown - 1, 0);
            AbilitySettings[abilityIndex].Item2.Text = AbilityResource[abilityIndex].CurrentCooldown.ToString();

            if(AbilityResource[abilityIndex].CurrentCooldown == 0)
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

    

    public void DisableAbility(int abilityIndex)
    {
        AbilitySettings[abilityIndex].Item1.Disabled = true;
        AbilitiesForceDisabled = true;
    }

    public void EnableAbility(int abilityIndex)
    {
        if (!AbilityResource[abilityIndex].OnCooldown)
        {
            AbilitySettings[abilityIndex].Item1.Disabled = false;
            AbilitySettings[abilityIndex].Item2.Text = "";
        }
        else
        {
            AbilitySettings[abilityIndex].Item2.Text = AbilityResource[abilityIndex].CurrentCooldown.ToString();
        }
        AbilitiesForceDisabled = false;
    }

}
