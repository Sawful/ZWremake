using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    static float player_range;
    public NavMeshAgent agent;
    public float rotateSpeedMovement = 0.05f;
    private float rotateVelocity;
    public float distanceWithEnemy;

    public Animator anim;
    float motionSmoothTime = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Attack();
    }

    public void Move()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                if(hit.collider.tag == "Ground")
                {
                    agent.SetDestination(hit.point);
                    agent.stoppingDistance = 0;
                }
            }
        }
    }
    public void Attack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Enemy")
                {
                    
                    distanceWithEnemy = Vector3.Distance(gameObject.transform.position, hit.collider.gameObject.transform.position);
                    print(PlayerScript.player_range);
                    print(distanceWithEnemy);
                    if (distanceWithEnemy > PlayerScript.player_range)
                    {
                        print("Out of range!");
                    }
                }
            }
        }
    }
}

