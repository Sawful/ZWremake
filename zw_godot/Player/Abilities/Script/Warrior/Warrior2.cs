using Godot;
using System;

public partial class Warrior2 : Ability
{
	float[] CooldownArray = new[] {22f, 21f, 20f, 19f, 18f}; // StatsListsFloat[0]
	float[] AttackSpeedArray = new[] {0.3f, 0.4f, 0.5f, 0.6f, 0.7f}; // StatsListsFloat[1]
	float[] DamageArray = new[] {0.1f, 0.15f, 0.2f, 0.25f, 0.3f}; // StatsListsFloat[2]
	public override void _Ready()
	{
		base._Ready();
		StatListsFloat.Add(CooldownArray);
		StatListsFloat.Add(AttackSpeedArray);
		StatListsFloat.Add(DamageArray);
	}

	public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        AbilityResource.Cooldown = StatListsFloatCurrent[0];
    }

	public void CastAbility(Entity caster)
	{
		CreateStatEffect(5, "AttackSpeed", StatListsFloatCurrent[1]);
        CreateStatEffect(5, "Damage", StatListsFloatCurrent[2]);

        Handler.AbilityUI.SetAbilityCooldown(1);
	}
}
