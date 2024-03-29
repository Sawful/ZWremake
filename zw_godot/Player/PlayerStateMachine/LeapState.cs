using Godot;
using System;
using System.Collections.Generic;

public partial class LeapState : SimpleState
{
	SimpleStateMachine StateMachine;
    private AbilityUI AbilityUI;
	private AbilityHandler AbilityHandler;
	Enemy Target;
    Player Player;
	Dictionary<string, object> Message;
    PackedScene TargetCircle;
    Node3D TargetCircleObject;
    
    public override void _Ready()
    {
        base._Ready();
		StateMachine = (SimpleStateMachine)GetParent().GetParent();
		TargetCircle = (PackedScene)ResourceLoader.Load("res://Enemies/TargetCircle.tscn");
		Player = (Player)StateMachine.GetParent();
		AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");
		AbilityHandler = Player.GetNode<AbilityHandler>("Abilities");
    }
    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;
        

        Target = (Enemy)Message["Target"];

        TargetCircleObject = (Node3D)TargetCircle.Instantiate();
        Target.AddChild(TargetCircleObject);
		
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void UpdateState(double dt)
	{
		Vector3 targetPosition = Target.Position;
		Player.Position = Player.Position.MoveToward(targetPosition, 20 * Convert.ToSingle(dt));

		float range = (float)Message["Range"];

		if (Player.Position.DistanceTo(targetPosition) <= range)
		{
			Player.RotateTo(targetPosition, Entity.RotationWeight);
            //Play spell
            Player.DealDamage(Target, (int) Mathf.Round(Player.Damage * (float)Message["DamageMultiplier"]));
            AbilityUI.SetAbilityCooldown(3); // Set Cooldown

            //Reset attack
            
            Player.AttackReload = 0.25;
			
			
			Dictionary<string, object> message = new()
            {
                {"Target",  Target},
            };
            StateMachine.ChangeState("AttackingState", message);
		}

	}

	    public override void OnExit(string NextState)
    {
        if (IsInstanceValid(Target))
        {
            TargetCircleObject.QueueFree();
        }
		StatEffect effect = ((Ability)Message["AbilityNode"]).CreateStatEffect(0, "AttackSpeed", 0.1);
		((Warrior4)Message["AbilityNode"]).AttackSpeedEffects.Add(effect);
        base.OnExit(NextState);
    }
}
