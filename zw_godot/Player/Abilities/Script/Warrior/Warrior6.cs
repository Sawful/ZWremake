using Godot;
using System;

public partial class Warrior6 : LineAbility
{
	public void CastAbility(Entity caster)
	{
		float length = 10;
        float width = 2;
        Line(length, width, caster);        
	}
	
	public void Ability(Entity caster)
	{
		Vector3 newPosition = (CurrentHitbox.PointHit - Player.Position).Normalized() * CurrentHitbox.Scale.Z + Player.Position; 

		System.Collections.Generic.Dictionary<string, object> message = new()
                {
                    {"DashLocation",  newPosition},
                    {"Cooldown", 10},
                    {"AbilityNode", this},
					{"Range", 0.05f},
					{"NextState", "IdleState"},
					{"Speed", 30f}
                };

        Player.PlayerStateMachine.ChangeState("DashState", message);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
