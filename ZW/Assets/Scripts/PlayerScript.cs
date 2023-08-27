using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    static public int playerHitpoint = 50;
    static public int playerDamage = 5;
    static public float playerRange = 5;
    static public float playerSpeed = 3;
    static public float playerAttackspeed = 1.0F;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHitpoint <= 0)
        {
            Destroy(gameObject);
        }
    }

    void takeDamage(int damage)
    {
        print(playerHitpoint);
        playerHitpoint -= damage;
    }
}
