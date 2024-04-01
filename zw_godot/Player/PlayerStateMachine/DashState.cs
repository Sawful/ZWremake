using Godot;
using System;
using System.Collections.Generic;

public partial class DashState : SimpleState
{
	SimpleStateMachine StateMachine;
    private AbilityUI AbilityUI;
	private AbilityHandler AbilityHandler;
	Enemy Target;
    Player Player;
    bool DashOnTarget;
    Vector3 DashLocation;
	Dictionary<string, object> Message;
    PackedScene TargetCircle;
    Node3D TargetCircleObject;
    float Speed;
    float Range;
    
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
        
        DashOnTarget = Message.ContainsKey("Target");
        if(DashOnTarget)
        {
            Target = (Enemy)Message["Target"];
            TargetCircleObject = (Node3D)TargetCircle.Instantiate();
            Target.AddChild(TargetCircleObject);
        }
        else
        {
            DashLocation = (Vector3)Message["DashLocation"];
        }
        
        if(Message.ContainsKey("Speed"))
        {
            Speed = (float)Message["Speed"];
        }
        else {Speed = 20;}


        if(Message.ContainsKey("AbilityOnStart"))
        {
            
        }
		Range = (float)Message["Range"];
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void UpdateState(double dt)
	{
        if(DashOnTarget)
        {
            DashLocation = Target.Position;
        }

		if (Player.Position.DistanceTo(DashLocation) <= Range)
		{
            Dictionary<string, object> message = new();

            if(DashOnTarget)
            {
                //Reset attack
                Player.AttackReload = 0.25;
                
                message.Add("Target",  Target);
            }
            StateMachine.ChangeState((string)Message["NextState"], message);
		}

        else
        {
		    Player.Position = Player.Position.MoveToward(DashLocation, Speed * Convert.ToSingle(dt));
        }

	}

	    public override void OnExit(string NextState)
    {

        if(Message.ContainsKey("AbilityOnExit"))
        {
            if((string)Message["AbilityOnExit"] == "Leap")
            {
                Player.RotateTo(DashLocation, Entity.RotationWeight);
                //Play spell
                Player.DealDamage(Target, (int) Mathf.Round(Player.Damage * (float)Message["DamageMultiplier"]));
                AbilityUI.SetLeapCooldown(3); // Set Cooldown
		        StatEffect effect = ((Ability)Message["AbilityNode"]).CreateStatEffect(0, "AttackSpeed", 0.1);
		        ((Warrior4)Message["AbilityNode"]).AttackSpeedEffects.Add(effect);
            }
        }
        if(DashOnTarget)
        {
            if (IsInstanceValid(Target))
            {
                TargetCircleObject.QueueFree();
            }
        }
        base.OnExit(NextState);
    }
}
