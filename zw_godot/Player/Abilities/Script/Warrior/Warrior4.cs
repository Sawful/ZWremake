using Godot;
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class Warrior4 : PointAndClickAbility
{

	Godot.Timer AutoAttackEndTimer;
	public List<StatEffect> AttackSpeedEffects = new();
    int AutoAttacksLaunched;

	public override void _Ready()
	{
        base._Ready();
		AutoAttackEndTimer = GetNode<Godot.Timer>("AutoAttackEndTimer");
	}

	public override void _Process(double delta)
	{
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
                        {"DamageMultiplier", 2f},
                        {"Cooldown", 10},
                        {"AbilityNode", this}
                    };
                    Player.PlayerStateMachine.ChangeState("AttackingState", message);
                }
            }
    }
   
    public void AutoAttacked() //Not activated
    {
        AutoAttacksLaunched++;
        AutoAttackEndTimer.WaitTime = 3;
        AutoAttackEndTimer.Start();
        Handler.AbilityUI.UpdateLeapCooldown("Ability4");
    }

}
