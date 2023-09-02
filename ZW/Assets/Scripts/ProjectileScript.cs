using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileScript : MonoBehaviour
{
    GameObject player;
    GameObject enemy;
    PlayerScript playerScript;
    MoveAndAttack moveAndAttack;
    float speed;
    public GameObject hitParticle;
    public AudioSource hitSound;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerScript>();
        moveAndAttack = player.GetComponent<MoveAndAttack>();
        speed = playerScript.projectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy)
        {
            transform.LookAt(enemy.transform);
            transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
        }

        else { Destroy(gameObject); }
    }

    public void FindEnemy(GameObject enemyTarget)
    {
        enemy = enemyTarget;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == enemy)
        {
            enemy.GetComponent<BaseEnemyScript>().takeDamage(player, playerScript.damage);
            // VFX and SFX
            SpawnHitParticles(enemy.transform.position);
            moveAndAttack.PlayHitSound();
            Destroy(gameObject);
        }
    }

    void SpawnHitParticles(Vector3 Position)
    {
        Instantiate(hitParticle, Position, Quaternion.identity);
    }

}
