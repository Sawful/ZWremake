using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileScript : MonoBehaviour
{
    GameObject player;
    GameObject enemy;
    PlayerScript playerScript;
    float speed;
    NavMeshAgent agent;
    public GameObject hitParticle;
    public AudioSource hitSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerScript>();
        agent = GetComponent<NavMeshAgent>();
        speed = playerScript.projectileSpeed;
        print(speed);
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = enemy.transform.position;
    }

    public void FindEnemy(GameObject enemyTarget)
    {
        print("found");
        enemy = enemyTarget;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("collide");
        if (collision.gameObject == enemy)
        {
            enemy.GetComponent<BaseEnemyScript>().takeDamage(player, playerScript.damage);
            // VFX and SFX
            SpawnHitParticles(enemy.transform.position);
            hitSound.Play();
            Destroy(gameObject);
        }
    }

    void SpawnHitParticles(Vector3 Position)
    {
        Instantiate(hitParticle, Position, Quaternion.identity);
    }

}
