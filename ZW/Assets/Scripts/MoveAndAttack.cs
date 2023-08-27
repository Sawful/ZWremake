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
            agent.SetDestination(enemyClicked.transform.position);

            if (PlayerScript.playerRange >= Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position))
            {
                gameObject.transform.LookAt(enemyClicked.transform.position);
                agent.SetDestination(gameObject.transform.position);
                walkToEnemy = false;
                attacking = true;
            }
        }
        
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (attacking && attackCooldown < 0.05)
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
                    if (hit.collider.tag == "Ground")
                    {
                        agent.SetDestination(hit.point);
                        agent.stoppingDistance = 0;
                        walkToEnemy = false;
                        attacking = false;
                    }

                    if (hit.collider.tag == "Enemy")
                    {
                        enemyClicked = hit.collider.gameObject;
                        distanceWithEnemy = Vector3.Distance(gameObject.transform.position, enemyClicked.transform.position);

                        if (distanceWithEnemy > PlayerScript.playerRange)
                        {
                            walkToEnemy = true;
                        }

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

