using Godot;
using System;

public partial class Warrior2 : Ability
{
	public void CastAbility(Entity caster)
	{
		CreateStatEffect(5, "AttackSpeed", 0.5f);
        CreateStatEffect(5, "Damage", 0.2f);

        Handler.AbilityUI.SetAbilityCooldown(1);
	}
}
