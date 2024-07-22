using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public partial class Warrior1 : PointAndClickAbility
{

    private PlayerInfo PlayerInfo;
    int[] FlatDamageArray = new[] {10, 25, 45, 70, 100}; // StatListsInt[0]
    float[] PercentDamageArray = new[] {0.2f, 0.25f, 0.35f, 0.40f, 0.5f}; // StatListsFloat[0]
    float[] CooldownArray = new[] {12f, 11f, 10f, 9f, 8f}; // StatsListsFloat[1]

	public override void _Ready()
	{
        base._Ready();
        PlayerInfo = GetNode<PlayerInfo>("/root/PlayerInfo");
        StatListsInt.Add(FlatDamageArray);
        StatListsFloat.Add(PercentDamageArray);
        StatListsFloat.Add(CooldownArray);

        // Setting ability
        
        UpgradeAbility(0);
	}

	public void CastAbility(Entity caster)
    {
        PointAndClick(caster);
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        AbilityResource.Cooldown = StatListsFloatCurrent[1];
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
                        {"PercentDamage", StatListsFloatCurrent[0]},
                        {"FlatDamage", StatListsIntCurrent[0]}
                    };
                    Player.PlayerStateMachine.ChangeState("AttackingState", message);
                }
            }
    }
        
}
