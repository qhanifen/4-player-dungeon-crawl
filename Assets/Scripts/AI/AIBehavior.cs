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
    public int nextWaypoint = 1;
    public int endingWaypoint = 0;
    [HideInInspector] public float nextWaypointDistance = 3f;
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
    public delegate void AIEvent();
    public event AIEvent SpawnEvent;
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
        if (SpawnEvent != null)
        {
            SpawnEvent();
        }
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

    public virtual void GetTarget()
    {
        target = PlayerManager.instance.GetClosestHero(transform);
    }

    #region AStar Methods
    public virtual void StartPath()
    {
        GetTarget();
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void UpdatePath()
    {
        if (Time.time - lastRepath > repathRate && seeker.IsDone())
        {
            lastRepath = Time.time + repathRate * Random.value * .5f;
            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            StartPath();                      
        }
        
        if (path == null)
        {
            Debug.Log("No path, check for errors or wait until path is finished.");
            // We have no path to follow yet, so don't do anything
            return;
        }

        if (nextWaypoint > path.vectorPath.Count) return;        
        
        // Direction to the next waypoint        
        Vector3 dir = Vector3.zero;
        if (nextWaypoint < path.vectorPath.Count)
        {
            dir = (path.vectorPath[nextWaypoint] - transform.position).normalized;

            if ((transform.position - path.vectorPath[nextWaypoint]).sqrMagnitude < nextWaypointDistance * nextWaypointDistance)
            {
                nextWaypoint++;
            }
        }
        else
        {
            dir = (target.position - transform.position).normalized;
        }
        Move(dir);

        // The commented line is equivalent to the one below, but the one that is used
        // is slightly faster since it does not have to calculate a square root
        //if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
     }

    public void OnPathComplete(Path p)
    {
        p.Claim(this);
        if (!p.error)
        {
            if (path != null)
            {
                path.Release(this);
            }
            path = p;
            nextWaypoint = 1;
            endingWaypoint = path.vectorPath.Count;
        }
        else
        {
            Debug.Log(p.error);
            p.Release(this);
        }
    }    

    public void Move(Vector3 dir)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, transform.position.y, dir.z));
        float angle = Quaternion.Angle(transform.rotation, lookRotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, stats.turnSpeed);
        Debug.DrawLine(transform.position, transform.position + dir * 5, Color.blue);
        transform.position += transform.forward * Time.fixedDeltaTime * stats.speed;
        //Control flooring        
        /*RaycastHit hit;
        if(Physics.Raycast(transform.position + Vector3.up * 5, Vector3.down, out hit))
        {
            transform.position = new Vector3(transform.position. x, hit.point.y, transform.position.z);
        }*/
    }    
    #endregion

    #region Combat Methods

    public virtual void Attack()
    {
        return;
    }

    #endregion
}
