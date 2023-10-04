using Godot;
using System;
//using System.Collections;
//using System.Collections.Generic;


public partial class move_and_attack : Node3D
{
    // Nodes
    [Export] public RayCast3D rayCast3D;
    [Export] public Camera3D camera3D;
    [Export] public StaticBody3D ground;
    // Stats
    [Export] public int speed = 3;
    // Raycast layers
    [Export(PropertyHint.Layers3DPhysics)] public uint mouseColliderLayers;
    // Raycast lenght
    private const float rayLength = 1000.0f;
   // Vectors
    private Vector3 anchorPoint = Vector3.Zero;
    private Vector3 cameraLocalStartingPosition;
    // Player States
    private bool moving = false;

    public override void _Ready()
    {
        cameraLocalStartingPosition = ToLocal(camera3D.GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Basic movement
        LookAt(anchorPoint);
        if (moving)
        {

            Position = Position.MoveToward(anchorPoint, speed * Convert.ToSingle(delta));
            GD.Print(Position);
            if (Position == anchorPoint)
            {
                moving = false;
                GD.Print("stopped moving");
            }
        }
        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            GD.Print("is registering click");
            RayCast(eventMouseButton);
        }
    }


    public void RayCast(InputEventMouseButton r_click)
    {
        PhysicsRayQueryParameters3D query = new()
        {
            From = camera3D.ProjectRayOrigin(r_click.Position),
            To = camera3D.ProjectRayNormal(r_click.Position) * rayLength,
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = mouseColliderLayers,
        };


        var hitDictionary = GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (hitDictionary.Count > 0) 
        {
            var objectHit = hitDictionary["collider"].Obj;
            if (objectHit == ground)
            {
                moving = true;
                anchorPoint = (Vector3)hitDictionary["position"];
                GlobalPosition.DirectionTo((Vector3)anchorPoint);
            }
        }

    }
    //	// Initialize agent
    //	public NavMeshAgent agent;
    //	// Initialize movement stats
    //	public float rotateSpeedMovement = 0.05f;
    //	// Initialize usefull variables
    //	public float distanceWithEnemy;
    //	public float attackCooldown = 0;
    //	// Initialize player states
    //	bool walkToEnemy = false;
    //	bool attacking = false;
    //	bool attackClick = false;
    //	//Get player stats
    //	public PlayerScript playerScript;
    //	// X input
    //	bool pressedAttackMoveKey = false;
    //	// Initialize Objects and Components
    //	public GameObject enemyClicked;
    //	public Animator anim;
    //	public GameObject hitParticle;
    //	public AudioSource hitSound;
    //	public GameObject clickAnimation;
    //	public Collider[] possibleTargets;
    //	public BindKey keybind;
    //	public BaseEnemyScript enemyScript;
    //	public GameObject rangedProjectile;
    //	public GameObject projectile;
    //	// Enemy layermask for Attack Click
    //	[SerializeField] private LayerMask layermask;
    //	//
    //	private float closestDistance;
    //	private float attackClickEnemyDistance;
    //	private Collider closestAttackClickEnemy;
    //
    //	// Start is called before the first frame update
    //	void Start()
    //	{
    //		// Get components
    //		agent = gameObject.GetComponent<NavMeshAgent>();
    //		hitSound = GetComponent<AudioSource>();
    //		keybind = GetComponent<BindKey>();
    //		playerScript = GetComponent<PlayerScript>();   
    //	}
    //
    //	// Update is called once per frame
    //	void Update()
    //	{
    //		// Update move speed
    //		agent.speed = playerScript.speed;
    //
    //		// Check if X was pressed before
    //		if (Input.GetKeyDown(keybind.attackMoveKey))
    //		{
    //			pressedAttackMoveKey = true;
    //		}
    //		// If X was pressed and left click is pressed:
    //		if (Input.GetMouseButtonDown(0) && pressedAttackMoveKey)
    //		{
    //			attackClick = true;
    //		}
    //
    //		// Move or attack function
    //		AttackMove();
    //
    //		if (walkToEnemy)
    //		{
    //			// When enemy is out of range
    //			if (playerScript.range < Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position))
    //			{
    //				agent.SetDestination(enemyClicked.transform.position);
    //			}
    //
    //			// When enemy is in range
    //			else
    //			{
    //				gameObject.transform.LookAt(enemyClicked.transform.position);
    //				agent.SetDestination(gameObject.transform.position);
    //				walkToEnemy = false;
    //				attacking = true;
    //			}
    //		}
    //		// AutoAttack
    //		playerScript.attackReload -= Time.deltaTime;
    //		if (attacking && playerScript.attackReload < 0)
    //		{
    //			enemyScript = enemyClicked.GetComponent<BaseEnemyScript>();
    //			// Range
    //			if (playerScript.rangedAttack)
    //			{
    //				projectile = Instantiate(rangedProjectile, transform.position, Quaternion.identity);
    //				playerScript.attackReload = 1 / playerScript.attackSpeed;
    //				projectile.GetComponent<ProjectileScript>().FindEnemy(enemyClicked);
    //			}
    //			// Melee
    //			else
    //			{
    //				enemyClicked.GetComponent<BaseEnemyScript>().takeDamage(gameObject, playerScript.damage);
    //				// VFX and SFX
    //				SpawnHitParticles(enemyClicked.transform.position);
    //
    //				//Set cooldown
    //				playerScript.attackReload = 1 / playerScript.attackSpeed;
    //			}
    //
    //
    //			// Is enemy dead
    //			if (enemyScript.health <= 0)
    //			{
    //				attacking = false;
    //			}
    //
    //		}
    //
    //		void AttackMove()
    //		{
    //			// Check all targets in range >> possibleTargets
    //			possibleTargets = Physics.OverlapSphere(gameObject.transform.position, playerScript.range, layermask);
    //			// Right click
    //			if (Input.GetMouseButtonDown(1))
    //			{
    //				attackClick = false;
    //				pressedAttackMoveKey = false;
    //
    //				RaycastHit hit;
    //
    //
    //				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
    //				{
    //					// When the raycast hit an Ground object 
    //					if (hit.collider.tag == "Ground")
    //					{
    //						agent.SetDestination(hit.point);
    //						agent.stoppingDistance = 0;
    //						walkToEnemy = false;
    //						attacking = false;
    //						Instantiate(clickAnimation, hit.point, Quaternion.identity * Quaternion.Euler(-90f, 0f, 0f));
    //					}
    //					// When the raycast hits an Enemy object
    //					if (hit.collider.tag == "Enemy")
    //					{
    //						enemyClicked = hit.collider.gameObject;
    //						distanceWithEnemy = Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position);
    //						// Out of range --> Walks up to the enemy
    //						if (distanceWithEnemy > playerScript.range)
    //						{
    //							walkToEnemy = true;
    //						}
    //						// In range --> Stop an attack the enemy
    //						else
    //						{
    //							agent.SetDestination(gameObject.transform.position);
    //							gameObject.transform.LookAt(enemyClicked.transform.position);
    //							attacking = true;
    //						}
    //					}
    //				}
    //			}
    //			// Attack click
    //			if (Input.GetMouseButtonDown(0) && pressedAttackMoveKey)
    //			{
    //				pressedAttackMoveKey = false;
    //				RaycastHit hit;
    //
    //				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
    //				{
    //					// When the raycast hit an Ground object 
    //					if (hit.collider.tag == "Ground")
    //					{
    //						agent.SetDestination(hit.point);
    //						agent.stoppingDistance = 0;
    //						walkToEnemy = false;
    //						attacking = false;
    //						Instantiate(clickAnimation, hit.point, Quaternion.identity * Quaternion.Euler(-90f, 0f, 0f));
    //					}
    //					// When the raycast hits an Enemy object
    //					if (hit.collider.tag == "Enemy")
    //					{
    //						enemyClicked = hit.collider.gameObject;
    //						distanceWithEnemy = Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position);
    //						// Out of range --> Walks up to the enemy
    //						if (distanceWithEnemy > playerScript.range)
    //						{
    //							walkToEnemy = true;
    //						}
    //						// In range --> Stop an attack the enemy
    //						else
    //						{
    //							agent.SetDestination(gameObject.transform.position);
    //							gameObject.transform.LookAt(enemyClicked.transform.position);
    //							attacking = true;
    //						}
    //					}
    //				}
    //				// When enemy in range
    //				if (possibleTargets != null && possibleTargets.Length != 0)
    //				{
    //					closestDistance = Mathf.Infinity;
    //
    //					foreach (Collider i in possibleTargets)
    //					{
    //						attackClickEnemyDistance = Vector3.Distance(gameObject.transform.position, i.transform.position);
    //						if (attackClickEnemyDistance < closestDistance)
    //						{
    //							closestDistance = attackClickEnemyDistance;
    //							closestAttackClickEnemy = i;
    //							Console.WriteLine(attackClickEnemyDistance);
    //						}
    //					}
    //
    //					// Attack the first enemy detected
    //					enemyClicked = closestAttackClickEnemy.gameObject;
    //					attacking = true;
    //					// then Stop
    //					agent.SetDestination(gameObject.transform.position);
    //					gameObject.transform.LookAt(enemyClicked.transform.position);
    //				}
    //			}
    //			// After attack click, if enemy in range
    //			else if (attackClick == true && possibleTargets != null && possibleTargets.Length != 0 && playerScript.attackReload < 0)
    //			{
    //				closestDistance = Mathf.Infinity;
    //
    //				foreach (Collider i in possibleTargets)
    //				{
    //					attackClickEnemyDistance = Vector3.Distance(gameObject.transform.position, i.transform.position);
    //					if (attackClickEnemyDistance < closestDistance)
    //					{
    //						closestDistance = attackClickEnemyDistance;
    //						closestAttackClickEnemy = i;
    //						Console.WriteLine(attackClickEnemyDistance);
    //					}
    //				}
    //
    //				// Attack the first enemy detected
    //				enemyClicked = closestAttackClickEnemy.gameObject;
    //				attacking = true;
    //				// then Stop movement
    //				agent.SetDestination(gameObject.transform.position);
    //				gameObject.transform.LookAt(enemyClicked.transform.position);
    //			}
    //		}
    //	}
    //
    //	void SpawnHitParticles(Vector3 Position)
    //	{
    //		Instantiate(hitParticle, Position, Quaternion.identity);
    //	}
    //
    //	public void PlayHitSound()
    //	{
    //		hitSound.Play();
    //	}
}
