using Godot;
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class Warrior4 : PointAndClickAbility
{
	float[] CooldownArray = new[] {12f, 11f, 10f, 9f, 8f}; // StatsListsFloat[0]
    float[] PercentDamageArray = new[] {0.1f, 0.1f, 0.15f, 0.15f, 0.2f}; // StatListsFloat[1]
    int[] FlatDamageArray = new[] {10, 20, 30, 40, 50}; // StatListsInt[0]

	Godot.Timer AutoAttackEndTimer;
	public List<StatEffect> AttackSpeedEffects = new();
    int AutoAttacksLaunched;

	public override void _Ready()
	{
        base._Ready();
		AutoAttackEndTimer = GetNode<Godot.Timer>("AutoAttackEndTimer");
        StatListsFloat.Add(CooldownArray);
		StatListsFloat.Add(PercentDamageArray);
		StatListsInt.Add(FlatDamageArray);
	}

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        AbilityResource.Cooldown = StatListsFloatCurrent[0];
    }

	private void _on_auto_attack_end_timer_timeout()
	{
        foreach (StatEffect effect in AttackSpeedEffects)
        {
            effect.Exit();
        }
        AttackSpeedEffects.Clear();
        AutoAttackEndTimer.Stop();
	}

	public void CastAbility(Entity caster)
    {
        PointAndClick(caster);
    }

    public void Ability(Entity caster)
    {
        Node3D node = (Node3D)AbilityRaycast()["objectHit"];

        if (node != null)
        {
            if ((Node3D)AbilityRaycast()["objectHit"] is Enemy enemyHit)
            {
                System.Collections.Generic.Dictionary<string, object> message = new()
                {
                    {"Target",  enemyHit},
                    {"Ability", "Leap"},
                    {"Range", 2f},
                    {"Leap Range", 10f},
                    {"PercentDamage", StatListsFloatCurrent[1]},
                    {"FlatDamage", StatListsIntCurrent[0]},
                    {"AbilityNode", this}
                };
                Player.PlayerStateMachine.ChangeState("AttackingState", message);
            }
        }
    }
   
    public void AutoAttacked()
    {
        AutoAttacksLaunched++;
        AutoAttackEndTimer.WaitTime = 3;
        AutoAttackEndTimer.Start();
        Handler.AbilityUI.UpdateLeapCooldown(3);
    }

}
