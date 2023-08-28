using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    public GameObject enemyToSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject spawn1 = GameObject.Find("SpawnLocation1");
        GameObject spawn2 = GameObject.Find("SpawnLocation2");
        GameObject spawn3 = GameObject.Find("SpawnLocation3");
        GameObject spawn4 = GameObject.Find("SpawnLocation4");

        Turn1(spawn1, spawn2, spawn3, spawn4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObject(GameObject obj, Vector3 pos, int number)
    {
        for (int i = 0; i < number; i++)
        { 
            GameObject newObject = Instantiate<GameObject>(obj, pos, Quaternion.identity);
        }
    }
    // Turn is cut down into a function to change turns easily
    void Turn1(GameObject spawn1, GameObject spawn2, GameObject spawn3, GameObject spawn4)
    {
        SpawnObject(enemyToSpawn, spawn1.transform.position, 3);
        SpawnObject(enemyToSpawn, spawn2.transform.position, 3);
        SpawnObject(enemyToSpawn, spawn3.transform.position, 3);
        SpawnObject(enemyToSpawn, spawn4.transform.position, 3);
    }
}
