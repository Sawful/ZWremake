using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    // Stats
    public int hitpoint = 50;
    public int damage = 2;
    public float speed = 1.5F;
    public float range = 0.5F;
    public float attackSpeed = 1.0F;
    public float attackCooldown = 0;

    // Initialize Objects and Component
    public GameObject player;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        // Get Objects and Component
        player = GameObject.Find("Player");
        agent = gameObject.GetComponent<NavMeshAgent>();
        // Set movespeed
        agent.speed = speed;

        // Range also adds size of player and enemy, which isn't the case for player attack.
        range += (gameObject.transform.localScale.x / 2) + (player.transform.localScale.x / 2);
        print(range);
    }

    // Update is called once per frame
    void Update()
    {
        // Move
        agent.SetDestination(player.transform.position);
        // Death state
        if (hitpoint <= 0)
        {
            Destroy(gameObject);
        }
        // Attack
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (range > Vector3.Distance(gameObject.transform.position, player.transform.position) && attackCooldown < 0.05)
        {
            player.SendMessage("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
            attackCooldown = 0.5F;
            print("hit");
        }
    }

    void takeDamage(int damage)
    {
        hitpoint -= damage;
    }
}
