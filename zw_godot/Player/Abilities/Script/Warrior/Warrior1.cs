using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public partial class Warrior1 : PointAndClickAbility
{
	public override void _Ready()
	{
        base._Ready();
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
                if (node is Enemy enemyHit)
                {
                    System.Collections.Generic.Dictionary<string, object> message = new()
                    {
                        {"Target",  enemyHit},
                        {"Ability", "Warrior1"},
                        {"Range", 2f},
                        {"DamageMultiplier", 2.5f}
                    };
                    Player.PlayerStateMachine.ChangeState("AttackingState", message);
                }
            }
    }
        
}
