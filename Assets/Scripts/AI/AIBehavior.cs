using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateController))]
public class AIBehavior : MonoBehaviour {

    #region AI Properties
    public StateController controller;    
    #endregion

    #region AStar Properties
    [HideInInspector] public Path path;
    [HideInInspector] public Seeker seeker;
    [HideInInspector] public float lastRepath = -9999f;
    [HideInInspector] public float repathRate = 0.5f;
    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public int currentWaypoint = 0;
    [HideInInspector] public float nextWaypointDistance = 3.0f;
    #endregion

    #region Public Properties
    [HideInInspector]
    public Animator anim;
    public bool running = false;
    public AIStats stats;
    public Transform target;
    public int currentHealth;
    public bool alerted = false;
    #endregion 

    // Use this for initialization
    void Start () {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<StateController>();
        controller.Initialize(this);
        anim = GetComponent<Animator>();

        currentHealth = stats.health;
	}
	
	// Update is called once per frame
	void Update ()
    {
        controller.Update();
	}

    #region AStar Methods
    public void OnPathComplete(Path p)
    {
        p.Claim(this);
        if (!p.error)
        {
            path = (MultiTargetPath)p;
            currentWaypoint = 0;
            UpdatePath();
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
        float speed = running ? stats.walkSpeed : stats.runSpeed;
        transform.position += dir * Time.fixedDeltaTime * speed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, transform.position.y, dir.z)), stats.turnSpeed);
    }

    public void UpdatePath()
    {
        Vector3[] pos = PlayerManager.instance.GetHeroPositions();
        seeker.StartMultiTargetPath(transform.position, pos, true);
    }    
    #endregion

    #region Combat Methods

    public virtual void Attack()
    {
        return;
    }

    #endregion
}
