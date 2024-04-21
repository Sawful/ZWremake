using Godot;
using System;
using System.Collections.Generic;

public partial class Warrior6 : LineAbility
{

	PackedScene AreaHitboxPacked;
    AreaHitbox AreaHitbox;
    List<Entity> TargetHit = new();
	bool Dashing = false;

	public override void _Ready()
    {
		base._Ready();
		AreaHitboxPacked = (PackedScene)ResourceLoader.Load("res://Player/Abilities/AreaHitbox.tscn");
	}

	public void CastAbility(Entity caster)
	{
		float length = 10;
        float width = 2;
        Line(length, width, caster);        
	}
	
	public void Ability(Entity caster)
	{
		Vector3 newPosition = (CurrentHitbox.PointHit - Player.Position).Normalized() * CurrentHitbox.Scale.Z + Player.Position; 

		Dictionary<string, object> message = new()
                {
                    {"DashLocation",  newPosition},
                    {"Cooldown", 10},
                    {"AbilityNode", this},
                    {"AbilityOnExit", "Warrior6"},
					{"Range", 0.05f},
					{"NextState", "IdleState"},
					{"Speed", 30f}
                };

        Player.PlayerStateMachine.ChangeState("DashState", message);
		
		AreaHitbox = (AreaHitbox)AreaHitboxPacked.Instantiate();
        AreaHitbox.FollowMouse = false;
		AreaHitbox.Scale = Vector3.One * 2;
        Player.AddChild(AreaHitbox);
		Dashing = true;
	}

	public override void _Process(double delta)
    {
		if(Dashing)
		{
			foreach(Entity Target in AreaHitbox.GetOverlappingBodies())
			{
				if(!TargetHit.Contains(Target))
				{
					Player.DealDamage(Target, Player.Damage);
					TargetHit.Add(Target);
				}
			}

		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void EndAbility()
	{
		AreaHitbox.QueueFree();
		TargetHit = new();
		Dashing = false;
	}
}
