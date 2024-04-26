using Godot;
using System;
using System.Collections.Generic;

public partial class Warrior7 : Ability
{
	public void CastAbility(Entity caster)
	{
		Dictionary<string, object> message = new()
        {
            {"Duration", 10d}
        };

		CreateStatEffect(10, "DamageReceived", -0.5f);
		CreateStatEffect(10, "AttackSpeed", 0.8f);
		CreateStatEffect(10, "MovementSpeed", 0.6f);
		Player.PlayerStateMachine.ChangeState("FrenzyState", message);

        Handler.AbilityUI.SetAbilityCooldown(3);
	}
}
