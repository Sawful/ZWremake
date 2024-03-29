using Godot;
using System;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class Flamestorm : AreaAbility
{
	public void CastAbility(Entity caster)
    {
        Area(2, caster);
    }

    public void Ability(Entity caster)
    { 
        Array<Node3D> targets = CurrentHitbox.GetOverlappingBodies();
        foreach (Entity target in targets)
        {
            caster.DealDamage(target, caster.Damage);
        }

        CurrentHitbox.QueueFree();

        Handler.AbilityUI.SetAbilityCooldown(1);
        Player.PlayerStateMachine.ChangeState("IdleState");
    }
}
