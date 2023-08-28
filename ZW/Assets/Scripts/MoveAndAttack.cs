using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    // Initialize Objects and Components
    public GameObject enemyClicked;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Get agent
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update move speed
        agent.speed = PlayerScript.playerSpeed;

        AttackMove();

        if (walkToEnemy)
        {
            // When enemy is out of range
            if (PlayerScript.playerRange < Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position))
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
        attackCooldown -= Time.deltaTime;
        if (attacking && attackCooldown < 0)
        {
            enemyClicked.SendMessage("takeDamage", PlayerScript.playerDamage, SendMessageOptions.DontRequireReceiver);
            attackCooldown = 1 / PlayerScript.playerAttackspeed;
        }


        void AttackMove()
        {
            if (Input.GetMouseButtonDown(1))
            {
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
                    }
                    // When the raycast hits an Enemy object
                    if (hit.collider.tag == "Enemy")
                    {
                        enemyClicked = hit.collider.gameObject;
                        distanceWithEnemy = Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position);
                        // Out of range --> Walks up to the enemy
                        if (distanceWithEnemy > PlayerScript.playerRange)
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
        }
    }
}

