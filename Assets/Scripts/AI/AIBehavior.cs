using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(StateController), typeof(Seeker))]
public class AIBehavior : MonoBehaviour
{

    #region AI Properties
    public StateController controller;
    #endregion

    #region AStar Properties
    [HideInInspector] public Path path;
    [HideInInspector] public Seeker seeker;
    [HideInInspector] public float lastRepath = -9999f;
    [HideInInspector] public float repathRate = .5f;
    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public int currentWaypoint = 0;
    [HideInInspector] public float nextWaypointDistance = .5f;
    #endregion

    #region Public Properties
    [HideInInspector]
    public Animator anim;
    public bool running = false;
    public AIStats stats;
    public Transform target;
    public int currentHealth;
    public float attackRange;
    public bool alerted = false;
    #endregion 

    // Use this for initialization
    void Start ()
    {      
        currentHealth = stats.health;
        anim = GetComponent<Animator>();
        seeker = GetComponent<Seeker>();

        //Initialize State Machine
        controller = GetComponent<StateController>();
        controller.Initialize(this);    
	}

    void OnDisable()
    {
        if (seeker.pathCallback != null)
        {
            seeker.pathCallback -= OnPathComplete;
        }
    }

    // Update is called once per frame
    void Update()
    {
        controller.UpdateController();        
    }
    
    #region AStar Methods
    public virtual void StartPath()
    {
        seeker.StartPath(transform.position, targetPosition);
    }        

    public void UpdatePath()
    {
        if (Time.time - lastRepath > repathRate && seeker.IsDone())
        {
            lastRepath = Time.time + Random.value * repathRate * 0.5f;
            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }
        if (path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }

        Vector3 dir = Vector3.zero;
        if (currentWaypoint > path.vectorPath.Count) return;
        if (currentWaypoint == path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            currentWaypoint++;

            //Switch to Idle state for a random period of time then calculate a new path
            return;
        }
        // Direction to the next waypoint
        if (currentWaypoint < path.vectorPath.Count)
        {
            dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        }
        else
        {
            dir = (targetPosition - transform.position).normalized;
        }
        
        Move(dir);

        // The commented line is equivalent to the one below, but the one that is used
        // is slightly faster since it does not have to calculate a square root
        //if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
        if ((transform.position - path.vectorPath[currentWaypoint]).sqrMagnitude < nextWaypointDistance * nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    public void OnPathComplete(Path p)
    {
        p.Claim(this);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            StartPath();
            p.Release(this);
        }
        else
        {
            Debug.Log(p.error);
            p.Release(this);
        }
    }    

    public void Move(Vector3 dir)
    {
        Debug.DrawLine(transform.position, transform.position + dir * 5, Color.blue);
        transform.position += dir * Time.fixedDeltaTime * stats.speed;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, transform.position.y, dir.z));
        float angle = Quaternion.Angle(transform.rotation, lookRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, angle / stats.turnSpeed);
    }    
    #endregion

    #region Combat Methods

    public virtual void Attack()
    {
        return;
    }

    #endregion
}
