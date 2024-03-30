using Godot;
using System;

public partial class Warrior2 : Ability
{
	public void CastAbility(Entity caster)
	{
		CreateStatEffect(5, "AttackSpeed", 0.5);
        CreateStatEffect(5, "Damage", 0.2);

        Handler.AbilityUI.SetAbilityCooldown(1);
	}
}
