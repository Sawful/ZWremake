using Godot;
using System;
using System.Collections.Generic;

public partial class DashState : ImmobileState
{

    private AbilityUI AbilityUI;
	private AbilityHandler AbilityHandler;
	Enemy Target;
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
		TargetCircle = (PackedScene)ResourceLoader.Load("res://Enemies/TargetCircle.tscn");
		AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");
		AbilityHandler = Player.GetNode<AbilityHandler>("Abilities");
    }
    
    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;

        if(Message.ContainsKey("AbilityOnStart"))
        {
        }



        Player.DisableAllAbilities();

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
            Player.CollisionMask = 4;
            Player.Targetable = false;
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
            if (Player.Position.DistanceTo(DashLocation) <= Range)
            {
                Dictionary<string, object> message = new();

                //Reset attack
                Player.AttackReload = 0.25;
                
                if (NextState != null) 
                {
                    StateMachine.ChangeState(NextState, NextStateMessage);
                }
                else 
                {
                    message.Add("Target",  Target);
                    StateMachine.ChangeState("AttackingState", message);
                }
            }

            else
            {
                Player.Position = Player.Position.MoveToward(DashLocation, Speed * Convert.ToSingle(dt));
            }
        }

        else
        {
            Player.Position = Player.Position.MoveToward(DashLocation, Speed * Convert.ToSingle(dt));

            if (Player.Position.DistanceTo(DashLocation) <= Range)
            {
                if (NextState != null) 
                {
                    StateMachine.ChangeState(NextState, NextStateMessage);
                }
                else 
                {
                    Dictionary<string, object> message = new();
                    StateMachine.ChangeState("IdleState", message);
                }
            }
        }
	}

	    public override void OnExit(string NextState)
    {

        Player.EnableAllAbilities();

        if(Message.ContainsKey("AbilityOnExit"))
        {
            if((string)Message["AbilityOnExit"] == "Leap")
            {
                Player.RotateTo(DashLocation, Entity.RotationWeight);
                //Play spell
                Player.DealDirectDamage(Target, (int) Mathf.Round(Player.Damage * (float)Message["DamageMultiplier"]));
                AbilityUI.SetLeapCooldown(3); // Set Cooldown
		        StatEffect effect = ((Ability)Message["AbilityNode"]).CreateStatEffect(0, "AttackSpeed", 0.1f);
		        ((Warrior4)Message["AbilityNode"]).AttackSpeedEffects.Add(effect);
            }

            else if((string)Message["AbilityOnExit"] == "Warrior6")
            {
                AbilityUI.SetAbilityCooldown(2); // Set Cooldown
                ((Warrior6)Message["AbilityNode"]).EndAbility();
            }
        }

        if(DashOnTarget)
        {
            if (IsInstanceValid(Target))
            {
                TargetCircleObject.QueueFree();
            }
        }

        else
        {
            Player.CollisionMask = 2 + 4 + 8;
            Player.Targetable = true;
        }
        base.OnExit(NextState);
    }
}
