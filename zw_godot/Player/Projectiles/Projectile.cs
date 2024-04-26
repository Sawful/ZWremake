using Godot;
using System;

public partial class Projectile : Area3D
{
	public Entity Target;
	public float Speed;
	public Entity ProjectileOwner;
	public int Damage;
	public bool DamageAbsolute = false;


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (!IsInstanceValid(Target))
        {
            QueueFree();
        }

		else 
		{
            Vector3 targetPos = Target.Position;

            float newRotationY = (float)Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(targetPos.X - Position.X, targetPos.Z - Position.Z), 0.8);
            Rotation = new Vector3(Rotation.X, newRotationY, Rotation.Z);

            Position = Position.MoveToward(targetPos, Speed * Convert.ToSingle(delta));
        }
        
    }

	public void OnBodyEntered(Entity body)
	{

		if (body == Target)
		{
			if(DamageAbsolute)
			{
            	ProjectileOwner.DealDamage(Target, Damage);
			}
			else
			{
				ProjectileOwner.DealDirectDamage(Target, Damage);
			}
			QueueFree();
        }
			

    }
}
