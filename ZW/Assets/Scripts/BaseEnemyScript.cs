using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyScript : MonoBehaviour
{
    [Header("Base Stats")]
    public int health;
    public int damage;
    public float range;
    public float speed;
    public float attackSpeed;
    public float attackReload = 0;

    [Header("Kill Reward")]
    public int expReward;
    public int goldReward;

    //Health Slider Variables
    public float damageLerpDuration;
    public float currentHealth;
    public float targetHealth;
    public Coroutine damageCoroutine;

    public HealthUI healthUI;

    public PlayerScript playerStats;

    // Initialize Objects and Component
    public GameObject player;
    public NavMeshAgent agent;

    private void Awake()
    {
        healthUI = GetComponent<HealthUI>();

        currentHealth = health;
        targetHealth = health;

        healthUI.Start3DSlider(health);
        healthUI.Update2DSlider(health, currentHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get Objects and Component
        player = GameObject.Find("Player");
        agent = gameObject.GetComponent<NavMeshAgent>();
        playerStats = GetComponent<PlayerScript>();
        // Set movespeed
        agent.speed = speed;

        // Range also adds size of player and enemy, which isn't the case for player attack.
        range += (gameObject.transform.localScale.x / 2) + (player.transform.localScale.x / 2);

        // Attack cooldown
        attackSpeed = attackReload;
    }


    // Update is called once per frame
    void Update()
    {
        //Enemy pattern 
        agent.SetDestination(player.transform.position);
        // Attack
        if (attackReload > 0)
        {
            attackReload -= Time.deltaTime;
        }

        if (range > Vector3.Distance(gameObject.transform.position, player.transform.position) && attackReload < 0.05)
        {
            player.GetComponent<PlayerScript>().takeDamage(gameObject, damage);
            attackReload = attackSpeed;
        }
    }
    public void takeDamage(GameObject attacker, float damageAmount)
    {
        targetHealth -= damageAmount;

        if (targetHealth <= 0)
        {
            attacker.GetComponent<PlayerScript>().killReward(expReward, goldReward);
            CheckIfEnemyDead();
            Destroy(gameObject);
        }
        else if (damageCoroutine == null)
        {
            StartLerpHealth();
        }

    }

    private void CheckIfEnemyDead()
    {
        healthUI.Update2DSlider(health, 0);
    }

    public void StartLerpHealth()
    {
        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(LerpHealth());
        }
    }

    private IEnumerator LerpHealth()
    {
        float elapsedTime = 0;
        float initialHealth = currentHealth;
        float target = targetHealth;

        while (elapsedTime < damageLerpDuration)
        {
            currentHealth = Mathf.Lerp(initialHealth, target, elapsedTime / damageLerpDuration);
            UpdateHealthUI();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentHealth = target;
        UpdateHealthUI();

        damageCoroutine = null;
    }

    private void UpdateHealthUI()
    {
        healthUI.Update2DSlider(health, currentHealth);
        healthUI.Update3DSlider(health, currentHealth);
    }

}