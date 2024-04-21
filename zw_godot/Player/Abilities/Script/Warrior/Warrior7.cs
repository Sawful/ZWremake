using Godot;
using System;
using System.Collections.Generic;

public partial class Warrior7 : Ability
{
	public void CastAbility(Entity caster)
	{
		CreateStatEffect(10, "DamageReceived", -0.5f);
		CreateStatEffect(10, "AttackSpeed", 0.8f);

        Handler.AbilityUI.SetAbilityCooldown(3);
	}
}
