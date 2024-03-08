using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public partial class Warrior1 : PointAndClickAbility
{
	// Called when the node enters the scene tree for the first time.
	    public override void _Ready()
	{
        base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
                        {"DamageMultiplier", 2.5f},
                        {"Cooldown", 10 * Handler.CooldownReduction}
                    };
                    Player.PlayerStateMachine.ChangeState("AttackingState", message);
                }
            }
    }
        
}
