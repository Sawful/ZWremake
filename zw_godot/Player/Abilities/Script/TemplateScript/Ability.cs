using Godot;
using System.Threading.Tasks;



public partial class Ability : Node
{
    public Player Player;
    public Camera3D MainCamera;
    public StaticBody3D Ground;
    public Node3D Main;
    public AbilityHandler Handler;
    public float Cooldown;

    private const float RayLength = 1000.0f;

    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;


    PackedScene StatEffect;

    public override void _Ready()
	{
        Handler = GetParent<AbilityHandler>();
        Player = Handler.GetParent<Player>();
        Ground = Player.Ground;
        Main = Player.GetParent<Node3D>();

        StatEffect = (PackedScene)ResourceLoader.Load("res://Player/Abilities/StatEffect.tscn");
    }

    public virtual System.Collections.Generic.Dictionary<string, object> AbilityRaycast()
    {
        MainCamera = Player.MainCamera;
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
    

    public StatEffect CreateStatEffect(float duration, string statEffect, double effectAmount)
    {
        StatEffect effect = (StatEffect)StatEffect.Instantiate();
        effect.Player = Player;
        effect.Message = new()
        {
            {"Duration", duration},
            {"StatEffect", statEffect},
            {"EffectAmount", effectAmount}
        };

        Player.AddChild(effect);
        return effect;
    }

    public async void AbilityStructure()
    {
        // Change cursor

        Handler.AbilityCast.SetResult(false); // Cancel other abilities
        Handler.AbilityCast = new TaskCompletionSource<bool>(); // Reset task to await cast confirmation
        Handler.Casting = true;

        if (await Handler.AbilityCast.Task == true) // Ability casted
        {
            // Cursor goes back to normal
            
            Handler.AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Handler.Casting = false;// Reset casting boolean
        }

        else // Ability canceled
        {
            // Cursor goes back to normal
            
            Handler.AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Handler.Casting = false; // Reset casting boolean
        }
    }

    public void SetTask()
    {
        Handler.AbilityCast.SetResult(false);
        Handler.AbilityCast = new TaskCompletionSource<bool>();
        Handler.Casting = true;
    }

    public void EndTask()
    {
        Handler.AbilityCast = new TaskCompletionSource<bool>();
        Handler.Casting = false;
    }
}

