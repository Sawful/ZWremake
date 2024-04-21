using Godot;
using System;
using Godot.Collections;

public partial class Warrior5 : AreaAbility
{

	public override void _Ready()
	{
		base._Ready();
		float radius = 3;

		CurrentHitbox = (AreaHitbox)AreaHitbox.Instantiate();
		CurrentHitbox.FollowMouse = false;
		CurrentHitbox.Scale = 2 * radius * Vector3.One;
		Player.AddChild(CurrentHitbox);
		GD.Print(CurrentHitbox);

	}

	public void CastAbility(Entity caster)
	{
        Array<Node3D> targets = CurrentHitbox.GetOverlappingBodies();

        CreateStatEffect(0.5f, "MovementSpeed", -0.2f);

		foreach (Node target in targets)
        {
			GD.Print(target);
        }

		foreach (Entity target in targets)
        {
			GD.Print(target);
            caster.DealDamage(target, caster.Damage);
        }

        CurrentHitbox.QueueFree();

        Handler.AbilityUI.SetAbilityCooldown(1);

		float radius = 3;
		CurrentHitbox = (AreaHitbox)AreaHitbox.Instantiate();
		CurrentHitbox.FollowMouse = false;
		CurrentHitbox.Scale = 2 * radius * Vector3.One;
		Player.AddChild(CurrentHitbox);
	}
}
