using Godot;
using System;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class Arrowshot : LineAbility
{
	public void CastAbility(Entity caster)
    {
        Line(4, 1, caster);
    }

    public void Ability(Entity caster)
    {
        Array<Node3D> targets = CurrentHitbox.GetOverlappingBodies();
        GD.Print(targets);
        foreach (Entity target in targets)
        {
            caster.DealDamage(target, caster.Damage);
            GD.Print("enemy hit with arrowshot");
        }

        CurrentHitbox.QueueFree();

        Handler.AbilityUI.SetAbilityCooldown(2);
        Player.PlayerStateMachine.ChangeState("IdleState");
    }
}
