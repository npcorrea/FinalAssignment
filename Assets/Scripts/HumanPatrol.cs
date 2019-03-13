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
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    public LayerMask PlayerMask;
    public LayerMask Default;
    public Transform[] waypoints;
    public float rotationSpeed = 2f;
    public GameObject player;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private bool foundPlayer = false;
    private bool m_isGrounded;
    private bool playerWasFound;
    private float m_speed;
    private float dist;


    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        m_speed = agent.speed;
        m_isGrounded = true;
        m_animator.SetBool("Grounded", m_isGrounded);
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

        //randomize desitination
        destPoint = Random.Range(0, waypoints.Length);

        // Set the agent to go to the currently selected destination.
        agent.destination = waypoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        //destPoint = (destPoint + 1) % waypoints.Length;
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        foundPlayer = false;
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
                    foundPlayer = true;
                }
            }
        }
    }

    IEnumerator RotateTowards(float angle)
    {
        /*Vector3 lookRight = target.transform.position;
        Vector3 direction = (lookRight - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        if (agent.transform.position == lookRight)
        {
            agent.isStopped = false;
            yield return null;
            GotoNextPoint();
        }*/
        /*float speed = .1f;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle, 0), speed * Time.deltaTime);
        Debug.Log("Rotating");



        int i = 0;
        while(i < 45)
        {
            transform.Rotate(0, angle, 0, Space.Self);
            Debug.Log("Rotating");
            yield return new WaitForSeconds(.5f);
            i++;
        }*/
        //yield return null;

        float rot = 0f;
        float dir = 1f;
        float rotSpeed = 10f;

        while (rot < angle)
        {
            float step = Time.deltaTime * rotSpeed;
            transform.Rotate(new Vector3(0, 12, 0) * step * dir);
            yield return null;
        }
            rot = 0f;
            dir *= -1f;
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
        // set human speed based on if they see cat or not
        if(foundPlayer == true)
        {
            agent.destination = player.transform.position;
            agent.speed = 1.2f;
            dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist < 2)
               player.transform.GetComponent<Rigidbody>().AddForce((player.transform.position - transform.position).normalized * -50);
        }
        else
        {
            agent.speed = .9f;
        }
        m_speed = agent.speed;
        m_animator.SetFloat("Speed_f", m_speed);

        // if player was being chased and then hid in the human's view
        if (playerWasFound == true && dist <= 8)
        {
            foundPlayer = true;
        }

        // Choose the next destination point when the agent gets close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            //agent.isStopped = true;
            //StartCoroutine("RotateTowards", 45f);
            //agent.isStopped = false;
            GotoNextPoint();


        }
        playerWasFound = foundPlayer;

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pushable") && collision.rigidbody.velocity.magnitude > 0)
        {
            Debug.Log("OW!");
        }
    }

}