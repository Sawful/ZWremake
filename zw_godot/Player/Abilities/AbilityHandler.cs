using Godot;
using System;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Godot.Collections;

public partial class AbilityHandler : Node
{
	public Player Player;
	public Node3D Main;
	public AbilityUI AbilityUI;
    public Camera3D MainCamera;
    public StaticBody3D Ground;
    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;
    private const float RayLength = 1000.0f;
	
    public CancellationTokenSource cancellationTokenSource;
    public bool Casting = false;
    public bool AttackMoving = false;
    public TaskCompletionSource<bool> AttackMoveContinue = new();
	public TaskCompletionSource<bool> AbilityCast = new();
	public bool Cast = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetParent<Player>();
        Ground = Player.Ground;
        Main = Player.GetParent<Node3D>();
		AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

        if ((Casting | Player.ClosestTarget != null) & AttackMoving) 
        {
            AttackMoveContinue.SetResult(true);
            AttackMoving = false;
        }

        
	}

	public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left && Casting == true)
        {
            AbilityCast.SetResult(true);
        }
    }

public async void AttackMove()
    {
        // Change cursor
        AbilityCast.SetResult(false); // Cancel other abilities

        AbilityCast = new TaskCompletionSource<bool>(); // Reset task to await cast confirmation
        Casting = true;

        if (await AbilityCast.Task == true) // Ability casted
        {
            // Cursor goes back to normal
            Casting = false; // Reset casting boolean
            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            AttackMoveContinue = new();
            AttackMoveContinue.SetResult(false);
            AttackMoveContinue = new();
            AttackMoving = true;

            System.Collections.Generic.Dictionary<string, object> info = AbilityRaycast();

            Vector3 movePoint = (Vector3)info["positionHit"];
            if(info["objectHit"] == Ground)
            {
                movePoint = NavigationServer3D.MapGetClosestPoint(NavigationServer3D.GetMaps()[0], (Vector3)AbilityRaycast()["positionHit"]);

                System.Collections.Generic.Dictionary<string, object> message = new()
                {
                    {"MovePoint",  movePoint}
                };

                Player.PlayerStateMachine.ChangeState("MovingState", message);

                if (await AttackMoveContinue.Task == true)
                {

                    if (Casting)
                    {
                        AttackMoveContinue = new();
                        AttackMoving = false;
                        return;
                    }

                    else if (Player.ClosestTarget != null)
                    {
                        System.Collections.Generic.Dictionary<string, object> message2 = new()
                        {
                            {"Target", Player.ClosestTarget},
                            {"Projectile Speed" ,  Player.ProjectileSpeed}
                        };
                        
                    if (Player.RangedAttack)
                        Player.PlayerStateMachine.ChangeState("RangeAttackingState", message2);

                    else
                        Player.PlayerStateMachine.ChangeState("AttackingState", message2);

                        AttackMoving = false;
                    }
                }
                
                else
                {
                    AttackMoveContinue = new();
                    AttackMoving = false;
                    return;
                }
            }
            
            else if(info["objectHit"] is Enemy)
            {
                System.Collections.Generic.Dictionary<string, object> message = new()
                {
                    {"Target",  info["objectHit"]}
                };
                Player.PlayerStateMachine.ChangeState("AttackingState", message);
            }
            
        }

        else // Ability canceled
        {
            // Cursor goes back to normal

            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Casting = false; // Reset casting boolean
            AttackMoveContinue = new();
            AttackMoving = false;
        }
    }
	public virtual System.Collections.Generic.Dictionary<string, object> AbilityRaycast()
    {
        Vector2 mouse_pos = GetViewport().GetMousePosition();
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = MainCamera.ProjectRayOrigin(mouse_pos),
            To = MainCamera.ProjectRayOrigin(mouse_pos) + MainCamera.ProjectRayNormal(mouse_pos) * RayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = MouseColliderLayers,
        };

        var hitDictionary = Player.GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0)
        {
            var objectHit = hitDictionary["collider"].Obj;
            var positionHit = hitDictionary["position"].Obj; 

            if  (objectHit == Ground | objectHit is Enemy)
            {
                System.Collections.Generic.Dictionary<string, object> info = new()
                {
                    { "objectHit", objectHit },
                    { "positionHit", positionHit }
                };

                return info;
            }
        }
        return null;
    }
}
