using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Base Stats")]
    public int health;
    public int damage;
    public float range;
    public float speed;
    // Attack speed is the number of attacks per second
    public float attackSpeed;
    public float attackReload = 0;

    [Header("Range attack")]
    public bool rangedAttack = true;
    public float projectileSpeed = 10;

    [Header("Experience")]
    public int level = 0;
    public int exp = 0;
    public int expUntilLevelUp = 2;

    [Header("Reward")]
    public int gold = 0;

    //Health Slider Variables
    public float damageLerpDuration;
    public float currentHealth;
    public float targetHealth;
    public Coroutine damageCoroutine;

    HealthUI healthUI;

    private void Awake()
    {
        healthUI = GetComponent<HealthUI>();

        currentHealth = health;
        targetHealth = health;

        healthUI.Start3DSlider(health);
        healthUI.Update2DSlider(health, currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void takeDamage(GameObject target, float damageAmount)
    {

        targetHealth -= damageAmount;

        if (targetHealth <= 0)
        {
            CheckIfPlayerDead();
            Destroy(gameObject);
        }
        else if (damageCoroutine == null)
        {
            StartLerpHealth();
        }

    }

    public void killReward(int expReward, int goldReward)
    {
        gold += goldReward;
        print("Gold: " + gold);
        gainExp(expReward);
    }

    public void gainExp(int expGained)
    {
        // Gain exp
        exp += expGained;

        // Check if Level up
        if (expUntilLevelUp <= exp) 
        {
            exp = 0;
            expUntilLevelUp += 1;
            level += 1;
        }
        print("Exp amount: " + exp);
        print("Exp till lvl up: " + expUntilLevelUp);
        print("Level: " + level);

    }

    private void CheckIfPlayerDead()
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