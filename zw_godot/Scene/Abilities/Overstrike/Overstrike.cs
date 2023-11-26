using Godot;
using System;

public partial class Overstrike : Node
{
	public static void Execute(Entity caster, Entity target = null)
	{
		if (target == null) { GD.Print("Overstrike casted"); }
		else { caster.DealDamage(target, caster.Damage * 10); }
		
	}
}
