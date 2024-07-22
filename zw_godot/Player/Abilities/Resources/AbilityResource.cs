using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityResource : Resource
{
	[Export] public string AbilityName;
	[Export] public int Slot;
	[Export] public bool TimedCooldown;
	[Export] public float Cooldown;
	[Export] public float CurrentCooldown;
	[Export] public bool OnCooldown;
	[Export] public bool CooldownRecharge;
	[Export] public bool MultipleCharges;
	[Export] public int MaxCharges;
	[Export] public int Charges;
	private Timer CooldownTimer;
	private PackedScene CooldownTimerPacked;
	public Ability AbilityNode;
	[Export] public string AbilityNodePath;
	[Export] public CompressedTexture2D Icon;
	public int AbilityLevel = 0;

	public AbilityUI AbilityUI;
	private List<Timer> CooldownTimerList = new();


	public void SetAbility(AbilityHandler abilityHandler)
	{
		PackedScene ability = (PackedScene)ResourceLoader.Load(AbilityNodePath);
		AbilityNode = (Ability)ability.Instantiate();
		AbilityNode.Cooldown = Cooldown;
		//AbilityNode.AbilityResource = this;
		abilityHandler.AddChild(AbilityNode);
		CooldownTimerPacked = (PackedScene)ResourceLoader.Load("res://Player/Abilities/Scenes/CooldownTimer.tscn");
		Charges = MaxCharges - CooldownTimerList.Count;
	}

	public void SetCooldown()
	{
		CooldownTimer = (Timer)CooldownTimerPacked.Instantiate();
		CooldownTimer.Timeout += CooldownEnd;
		CooldownTimer.WaitTime = Cooldown;
		AbilityNode.AddChild(CooldownTimer);

        CooldownTimerList.Add(CooldownTimer);

		CooldownRecharge = true;

		Charges = MaxCharges - CooldownTimerList.Count;

		if(Charges == 0)
		{
			OnCooldown = true;
		}
	}


	public void UpdateCooldown()
	{
		if(CooldownRecharge)
		{
			CurrentCooldown = Cooldown;

			foreach (Timer currentCooldownTimer in CooldownTimerList)
			{
				CurrentCooldown = (float)Mathf.Min(CurrentCooldown, currentCooldownTimer.TimeLeft);
			}
		}
	}

	public void CooldownEnd()
	{
		CooldownTimerList[0].QueueFree();
		CooldownTimerList.RemoveAt(0);

		Charges = MaxCharges - CooldownTimerList.Count;

		if(Charges == MaxCharges)
		{
			CooldownRecharge = false;
			AbilityNode.Handler.AbilityUI.ClearCooldown(Slot - 1);
		}

		OnCooldown = false;
		AbilityNode.Handler.AbilityUI.UnlockCooldown(Slot - 1);
	}
}
