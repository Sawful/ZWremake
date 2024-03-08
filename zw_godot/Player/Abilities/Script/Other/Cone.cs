using Godot;
using System;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class Cone : LineAbility
{
	public void CastAbility(Entity caster)
    {
        Cone(4, 3, caster);
    }

    public void Ability(Entity caster)
    {
        GD.Print("Cone casted");

        Array<Node3D> targets = CurrentHitbox.GetOverlappingBodies();
        foreach (Entity target in targets)
        {
            caster.DealDamage(target, caster.Damage);
        }

        Handler.AbilityUI.SetAbilityCooldown("Ability4", 10);
        Player.PlayerStateMachine.ChangeState("IdleState");
    }
}
