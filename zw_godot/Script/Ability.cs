using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Ability : Node
{
	public bool Cast = false;
    [Export] private RayCast3D RayCast3D;
    [Export] private Camera3D Camera3D;
    [Export] private StaticBody3D Ground;
    private Node3D Main;

    PackedScene AreaIndicator;

    public CancellationTokenSource cancellationTokenSource;

    private bool Casting = false;

    private const float RayLength = 1000.0f;

    [Export(PropertyHint.Layers3DPhysics)] public uint MouseColliderLayers;


    public Player Player;

    public TaskCompletionSource<bool> AbilityCast = new TaskCompletionSource<bool>();


    public override void _Ready()
	{
        AreaIndicator = (PackedScene)ResourceLoader.Load("res://Visual/Indicator/AreaIndicator.tscn");

        Player = GetParent<Player>();
        Camera3D = Player.Camera3D;
        Ground = Player.Ground;

        Main = Player.GetParent<Node3D>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left && Casting == true)
        {
            AbilityCast.SetResult(true);
        }

    }

    public virtual Node3D AbilityRaycast()
    {
        Vector2 mouse_pos = GetViewport().GetMousePosition();
        // Raycast
        PhysicsRayQueryParameters3D query = new()
        {
            From = Camera3D.ProjectRayOrigin(mouse_pos),
            To = Camera3D.ProjectRayNormal(mouse_pos) * RayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = MouseColliderLayers,
        };

        var hitDictionary = Player.GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0)
        {
            var objectHit = hitDictionary["collider"].Obj;

            if  (objectHit == Ground)
            {
                return (StaticBody3D)objectHit;
            }

            else if (objectHit is Enemy enemyHit)
            {
                return enemyHit;
            }
        }
        return null;
    }

    public async void Overstrike(Entity caster)
    {
        /// <summary>Point and click massive damage.</summary>

        // Change cursor

        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();
        Casting = true;

        if (await AbilityCast.Task == true)
        {
            Node3D node = AbilityRaycast();

            if (node != null)
            {
                GD.Print("Mouse at: " + node.Position.ToString());

                if (AbilityRaycast() is Enemy enemyHit)
                {
                    GD.Print("enemy found");

                    caster.DealDamage(enemyHit, caster.Damage * 10);
                }
            }

            // Cursor goes back to normal
            GD.Print("overstrike casted");
            AbilityCast = new TaskCompletionSource<bool>();
            Casting = false;

        }
        else
        {
            // Cursor goes back to normal
            GD.Print("overstrike cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
            Casting = false;
        }
    }

    public async void Flamestorm(Entity caster)
    {
        // Change cursor

        AbilityCast.SetResult(false);
        AbilityCast = new TaskCompletionSource<bool>();

        GD.Print(AreaIndicator);

        AreaIndicator AreaIndic = (AreaIndicator)AreaIndicator.Instantiate();
        Main.AddChild(AreaIndic);
        GD.Print(AreaIndic);

        if (await AbilityCast.Task == true)
        {
            AreaIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Flamestorm casted");
            AbilityCast = new TaskCompletionSource<bool>();

        }
        else
        {
            AreaIndic.QueueFree();
            // Cursor goes back to normal
            GD.Print("Flamestorm cancelled");
            AbilityCast = new TaskCompletionSource<bool>();
        }
    }

    public async void AbilityStructure()
    {
        // Change cursor

        AbilityCast.SetResult(false); // Cancel other abilities
        AbilityCast = new TaskCompletionSource<bool>(); // Reset task to await cast confirmation

        if (await AbilityCast.Task == true) // Ability casted
        {
            // Cursor goes back to normal
            
            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Casting = false;// Reset casting boolean
        }

        else // Ability canceled
        {
            // Cursor goes back to normal
            
            AbilityCast = new TaskCompletionSource<bool>(); // Reset task
            Casting = false; // Reset casting boolean
        }
    }
}

