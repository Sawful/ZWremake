using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MoveAndAttack : MonoBehaviour
{
    // Initialize agent
    public NavMeshAgent agent;
    // Initialize movement stats
    public float rotateSpeedMovement = 0.05f;
    // Initialize usefull variables
    public float distanceWithEnemy;
    public float attackCooldown = 0;
    // Initialize player states
    bool walkToEnemy = false;
    bool attacking = false;
    bool attackClick = false;
    //Get player stats
    public PlayerScript playerScript;
    // X input
    bool pressedAttackMoveKey = false;
    // Initialize Objects and Components
    public GameObject enemyClicked;
    public Animator anim;
    public GameObject hitParticle;
    public AudioSource hitSound;
    public GameObject clickAnimation;
    public Collider[] possibleTargets;
    public BindKey keybind;
    // Enemy layermask for Attack Click
    [SerializeField] private LayerMask layermask;
    //
    private float closestDistance;
    private float attackClickEnemyDistance;
    private Collider closestAttackClickEnemy;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        agent = gameObject.GetComponent<NavMeshAgent>();
        hitSound = GetComponent<AudioSource>();
        keybind = GetComponent<BindKey>();
        playerScript = GetComponent<PlayerScript>();    
    }

    // Update is called once per frame
    void Update()
    {
        // Update move speed
        agent.speed = playerScript.speed;

        // Check if X was pressed before
        if (Input.GetKeyDown(keybind.attackMoveKey))
        {
            pressedAttackMoveKey = true;
        }
        // If X was pressed and left click is pressed:
        if (Input.GetMouseButtonDown(0) && pressedAttackMoveKey)
        {
            attackClick = true;
        }

        // Move or attack function
        AttackMove();

        if (walkToEnemy)
        {
            // When enemy is out of range
            if (playerScript.range < Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position))
            {
                agent.SetDestination(enemyClicked.transform.position);
            }

            // When enemy is in range
            else
            {
                gameObject.transform.LookAt(enemyClicked.transform.position);
                agent.SetDestination(gameObject.transform.position);
                walkToEnemy = false;
                attacking = true;
            }
        }
        // AutoAttack
        playerScript.attackReload -= Time.deltaTime;
        if (attacking && playerScript.attackReload < 0)
        {
            enemyClicked.GetComponent<BaseEnemyStats>().takeDamage(gameObject, playerScript.damage);
            // VFX and SFX
            SpawnHitParticles(enemyClicked.transform.position);
            hitSound.Play();
            //Set cooldown
            playerScript.attackReload = playerScript.attackSpeed;

        }

        void AttackMove()
        {
            possibleTargets = Physics.OverlapSphere(gameObject.transform.position, playerScript.range, layermask);
            if (Input.GetMouseButtonDown(1))
            {
                attackClick = false;
                pressedAttackMoveKey = false;

                RaycastHit hit;


                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    // When the raycast hit an Ground object 
                    if (hit.collider.tag == "Ground")
                    {
                        agent.SetDestination(hit.point);
                        agent.stoppingDistance = 0;
                        walkToEnemy = false;
                        attacking = false;
                        Instantiate(clickAnimation, hit.point, Quaternion.identity * Quaternion.Euler(-90f, 0f, 0f));
                    }
                    // When the raycast hits an Enemy object
                    if (hit.collider.tag == "Enemy")
                    {
                        enemyClicked = hit.collider.gameObject;
                        distanceWithEnemy = Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position);
                        // Out of range --> Walks up to the enemy
                        if (distanceWithEnemy > playerScript.range)
                        {
                            walkToEnemy = true;
                        }
                        // In range --> Stop an attack the enemy
                        else
                        {
                            agent.SetDestination(gameObject.transform.position);
                            gameObject.transform.LookAt(enemyClicked.transform.position);
                            attacking = true;
                        }
                    }
                }
            }
            // Attack click
            if (Input.GetMouseButtonDown(0) && pressedAttackMoveKey)
            {
                pressedAttackMoveKey = false;
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    // When the raycast hit an Ground object 
                    if (hit.collider.tag == "Ground")
                    {
                        agent.SetDestination(hit.point);
                        agent.stoppingDistance = 0;
                        walkToEnemy = false;
                        attacking = false;
                        Instantiate(clickAnimation, hit.point, Quaternion.identity * Quaternion.Euler(-90f, 0f, 0f));
                    }
                    // When the raycast hits an Enemy object
                    if (hit.collider.tag == "Enemy")
                    {
                        enemyClicked = hit.collider.gameObject;
                        distanceWithEnemy = Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position);
                        // Out of range --> Walks up to the enemy
                        if (distanceWithEnemy > playerScript.range)
                        {
                            walkToEnemy = true;
                        }
                        // In range --> Stop an attack the enemy
                        else
                        {
                            agent.SetDestination(gameObject.transform.position);
                            gameObject.transform.LookAt(enemyClicked.transform.position);
                            attacking = true;
                        }
                    }
                }
                // When enemy in range
                if (possibleTargets != null && possibleTargets.Length != 0)
                {
                    closestDistance = Mathf.Infinity;

                    foreach (Collider i in possibleTargets)
                    {
                        attackClickEnemyDistance = Vector3.Distance(gameObject.transform.position, i.transform.position);
                        if (attackClickEnemyDistance < closestDistance)
                        {
                            closestDistance = attackClickEnemyDistance;
                            closestAttackClickEnemy = i;
                            Console.WriteLine(attackClickEnemyDistance);
                        }
                    }

                    // Attack the first enemy detected
                    enemyClicked = closestAttackClickEnemy.gameObject;
                    attacking = true;
                    // then Stop
                    agent.SetDestination(gameObject.transform.position);
                    gameObject.transform.LookAt(enemyClicked.transform.position);
                }
            }
            // After attack click, if enemy in range
            else if (attackClick == true && possibleTargets != null && possibleTargets.Length != 0 && playerScript.attackReload < 0)
            {
                closestDistance = Mathf.Infinity;

                foreach (Collider i in possibleTargets)
                {
                    attackClickEnemyDistance = Vector3.Distance(gameObject.transform.position, i.transform.position);
                    if (attackClickEnemyDistance < closestDistance)
                    {
                        closestDistance = attackClickEnemyDistance;
                        closestAttackClickEnemy = i;
                        Console.WriteLine(attackClickEnemyDistance);
                    }
                }

                // Attack the first enemy detected
                enemyClicked = closestAttackClickEnemy.gameObject;
                attacking = true;
                // then Stop movement
                agent.SetDestination(gameObject.transform.position);
                gameObject.transform.LookAt(enemyClicked.transform.position);
            }
        }
    }

    void SpawnHitParticles(Vector3 Position)
    {
        Instantiate(hitParticle, Position, Quaternion.identity);
    }
}