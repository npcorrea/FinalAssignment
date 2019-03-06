/*using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class HumanPatrol : MonoBehaviour
{

    public Transform[] waypoints;
    public GameObject eyes;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private float distance = 2;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;
        agent.isStopped = true;
        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (waypoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = waypoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    void Look()
    {

    }

    void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * distance, Color.green);

        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward, out hit, distance, LayerMask.GetMask("Default")))
        {
            if (hit.collider.tag == "Player")
                Debug.Log("Found Kitty");
            else
                return;
        }

        // Choose the next destination point when the agent gets close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            //agent.isStopped = true;
            //Look();
            GotoNextPoint();
        }
            
    }
}*/

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class HumanPatrol : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask PlayerMask;
    public LayerMask Default;
    public Transform[] waypoints;
    //public GameObject eyes;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private float distance = 2;


    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine("FindTargetsWithDelay", .2f);
        GotoNextPoint();
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (waypoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = waypoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, PlayerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position + new Vector3(0,1.0f,0), dirToTarget, dstToTarget, Default))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    void Look()
    {
        agent.transform.RotateAround(transform.position, transform.up, 45*Time.deltaTime);
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void Update()
    {
        // Choose the next destination point when the agent gets close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            //agent.isStopped = true;
            agent.speed = 0;
            //Look();
            //GotoNextPoint();
        }

    }

}