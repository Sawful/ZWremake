using Godot;
using System;
using System.Collections.Generic;

public partial class ImmobileState : PlayerState
{
	protected string NextState;
	protected Dictionary<string, object> NextStateMessage = new();

	public override void _Input(InputEvent @event)
    {

        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right && Enabled)
        {

			Godot.Collections.Dictionary hitDictionary = Player.RightClickRaycast(eventMouseButton);
            object objectHit = hitDictionary["collider"].Obj;

			if (objectHit == Player.Ground)
            {

                Vector3 AnchorPoint = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps()[0], (Vector3)hitDictionary["position"]);
                NextStateMessage = new()
                {
                    { "MovePoint", AnchorPoint }
                };

				NextState = "MovingState";
            }
            
            else if (objectHit is Enemy enemyHit)
            {

                Enemy enemyClicked = enemyHit;
                NextStateMessage = new()
                {
                    { "Target", enemyClicked },
                    { "Projectile Speed", Player.ProjectileSpeed }
                };

                if (Player.RangedAttack) 
                {
                    NextState = "RangedAttackingState";
                }
                    
                else
				{
					NextState = "AttackingState";
				}
                    
            }
        }
    }
}
