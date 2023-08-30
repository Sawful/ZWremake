using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemyInRange : MonoBehaviour
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        player.SendMessage("EnemyInRange", other, SendMessageOptions.DontRequireReceiver);
        print("in range");
    }
}
