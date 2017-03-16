using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Action/Patrol Action")]
public class PatrolAction : AIAction
{
    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateController controller)
    {
        if (Time.time - controller.AI.lastRepath > controller.AI.repathRate && controller.AI.seeker.IsDone())
        {
            controller.AI.lastRepath = Time.time + Random.value * controller.AI.repathRate * 0.5f;
            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            controller.AI.seeker.StartPath(controller.AI.transform.position, controller.AI.targetPosition, controller.AI.OnPathComplete);
        }
        if (controller.AI.path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }
        if (controller.AI.currentWaypoint > controller.AI.path.vectorPath.Count) return;
        if (controller.AI.currentWaypoint == controller.AI.path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            controller.AI.currentWaypoint++;
            
            //Switch to Idle state for a random period of time then calculate a new path

            return;
        }
        // Direction to the next waypoint
        Vector3 dir = (controller.AI.path.vectorPath[controller.AI.currentWaypoint] - controller.AI.transform.position).normalized;
        dir *= controller.AI.stats.walkSpeed;        
        controller.AI.Move(dir);
        // The commented line is equivalent to the one below, but the one that is used
        // is slightly faster since it does not have to calculate a square root
        //if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
        if ((controller.AI.transform.position - controller.AI.path.vectorPath[controller.AI.currentWaypoint]).sqrMagnitude < controller.AI.nextWaypointDistance * controller.AI.nextWaypointDistance)
        {
            controller.AI.currentWaypoint++;
            return;
        }
    }
}
