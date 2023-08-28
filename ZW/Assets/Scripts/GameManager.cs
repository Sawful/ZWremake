using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class GameManager : MonoBehaviour
{
    public GameObject enemy;
    static public int round = 0;
    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;
    public GameObject spawn4;

    // Start is called before the first frame update
    void Start()
    {
        spawn1 = GameObject.Find("SpawnLocation1");
        spawn2 = GameObject.Find("SpawnLocation2");
        spawn3 = GameObject.Find("SpawnLocation3");
        spawn4 = GameObject.Find("SpawnLocation4");

        Round1();
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

    void SpawnAllCorners(GameObject obj, int number)
    {
        SpawnObject(obj, spawn1.transform.position, number);
        SpawnObject(obj, spawn2.transform.position, number);
        SpawnObject(obj, spawn3.transform.position, number);
        SpawnObject(obj, spawn4.transform.position, number);
    }

    // Turn is cut down into a function to change turns easily
    void Round1()
    {
        StartCoroutine(waiter());
        round++;
        print(round);
        IEnumerator waiter()
        {
            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(5);

            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(5);

            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(10);

            Round2();
        }
    }

    void Round2()
    {
        StartCoroutine(waiter());
        round++;
        print(round);
        IEnumerator waiter()
        {
            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(1);

            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(1);

            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(1);

            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(1);

            SpawnAllCorners(enemy, 1);

            yield return new WaitForSeconds(1);
        }
    }
}
