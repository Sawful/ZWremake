using Godot;
using System;
using System.Collections.Generic;

public partial class MobileState : PlayerState
{

	public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right && Enabled)
        {
			Godot.Collections.Dictionary hitDictionary = Player.RightClickRaycast(eventMouseButton);
            object objectHit = hitDictionary["collider"].Obj;

			if (objectHit == Player.Ground)
            {
                Vector3 AnchorPoint = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps()[0], (Vector3)hitDictionary["position"]);
                Dictionary<string, object> message = new()
                {
                    { "MovePoint", AnchorPoint }
                };

                StateMachine.ChangeState("MovingState", message);
            }
            
            else if (objectHit is Enemy enemyHit)
            {
                Enemy enemyClicked = enemyHit;
                Dictionary<string, object> message = new()
                {
                    { "Target", enemyClicked },
                    { "Projectile Speed", Player.ProjectileSpeed }
                };

                if (Player.RangedAttack) 
                {
                    StateMachine.ChangeState("RangeAttackingState", message);
                }
                    
                else
                    StateMachine.ChangeState("AttackingState", message);
            }
        }
    }
}
