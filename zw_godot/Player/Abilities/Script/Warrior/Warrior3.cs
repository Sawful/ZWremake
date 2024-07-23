using Godot;
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class Warrior3 : LineAbility
{
	float[] CooldownArray = new[] {22f, 21f, 20f, 19f, 18f}; // StatsListsFloat[0]
    float[] PercentDamageArray = new[] {0.1f, 0.15f, 0.2f, 0.25f, 0.3f}; // StatListsFloat[1]
    int[] FlatDamageArray = new[] {4, 8, 12, 16, 20}; // StatListsInt[0]

	System.Timers.Timer AbilityTimer;
    public TaskCompletionSource<bool> AbilityTimerFinished = new();
    public override void _Ready()
	{
        base._Ready();
        StatListsFloat.Add(CooldownArray);
		StatListsFloat.Add(PercentDamageArray);
		StatListsInt.Add(FlatDamageArray);
	}

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        AbilityResource.Cooldown = StatListsFloatCurrent[0];
    }

	public void CastAbility(Entity caster)
    {
        float length = 4;
        float width = 3;
        Cone(length, width, caster);        
    }

	private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        AbilityTimerFinished.SetResult(true);
    }

    public async void Ability(Entity caster)
    {
        // loop
        AbilityTimer = new();
        AbilityTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        AbilityTimer.AutoReset = true;
        AbilityTimer.Interval = 333;
        AbilityTimer.Enabled = true;

        Player.StatsBonusMult["MovementSpeed"] += -0.5f;
        Player.UpdateStats();

        Handler.AbilityUI.SetAbilityCooldown(2);
        int loopNumber = 0;
        CurrentHitbox.PositionLocked = true;

        while (loopNumber != 6)
        {
            if (await AbilityTimerFinished.Task == true)
            {
                Array<Node3D> targets = CurrentHitbox.GetOverlappingBodies();
                foreach (Entity target in targets)
                {
                    int damage = (int)Math.Round(Player.Damage * StatListsFloatCurrent[1]) + StatListsIntCurrent[0];
                    Player.DealDirectDamage(target, damage);
                }

                AbilityTimerFinished = new TaskCompletionSource<bool>();

                loopNumber ++;
            }
        }
        Player.StatsBonusMult["MovementSpeed"] -= -0.5f;
        Player.UpdateStats();

        AbilityTimer.Enabled = false;
    }


}
