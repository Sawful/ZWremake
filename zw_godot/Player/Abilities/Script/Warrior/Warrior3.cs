using Godot;
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class Warrior3 : LineAbility
{
	System.Timers.Timer AbilityTimer;
    public TaskCompletionSource<bool> AbilityTimerFinished = new();
    public override void _Ready()
	{
        base._Ready();
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
                    Player.DealDamage(target, (int)Math.Round(Player.Damage * 0.66));
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
