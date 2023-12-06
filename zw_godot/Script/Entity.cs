using Godot;
using System;

public partial class Entity : RigidBody3D
{
    [Export] public int MaxHealth;
    [Export] public int Health;
    [Export] public int Damage;
    [Export] public float Range;
    [Export] public float Speed;
    [Export] public double AttackSpeed;
    [Export] public double AttackReload;

    public const float RotationWeight = 0.1f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Health = MaxHealth;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void RotateTo(Vector3 rotationPoint, float rotationWeight)
    {
        var newRotationY = Mathf.LerpAngle(Rotation.Y, Mathf.Atan2(rotationPoint.X - Position.X, rotationPoint.Z - Position.Z), rotationWeight);
        Rotation = new Vector3(Rotation.X, newRotationY, Rotation.Z);
    }

    public void MoveTo(double delta, Vector3 moveTo)
    {
        // Player update
        RotateTo(moveTo, RotationWeight);
        Position = Position.MoveToward(moveTo, Speed * Convert.ToSingle(delta));
    }

    public void RegenerateHealth(int amount)
    {
        Health = Math.Clamp(Health + amount, 0, MaxHealth);
    }


    public void DealDamage(Entity target, int damageAmount)
    {
        target.TakeDamage(this, damageAmount);
    }

    public void TakeDamage(Entity attacker, int damageAmount)
    {
        Health -= damageAmount;
        GD.Print(attacker.ToString() + "dealt: " + damageAmount.ToString() + " damage.");
        GD.Print("Current health:" + Health.ToString());
        CheckIfDead();
    }

    public void CheckIfDead()
    {
        if (Health <= 0)
        {
            this.QueueFree();
        }
    }

    public virtual Node LoadAbility(string name)
    {
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Scene/Abilities/" + name + "/" + name + ".tscn");
        Node sceneNode = scene.Instantiate();
        AddChild(sceneNode);
        return sceneNode;
    }

}
