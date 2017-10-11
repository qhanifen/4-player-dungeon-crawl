using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Chase Player")]
public class ChasePlayer : AIAction
{
    public override void Act(StateController controller)
    { 
        Chase(controller);
    }

    private void Chase(StateController controller)
    {
        controller.AI.GetTarget();
        if(controller.AI.path == null)
        {
            controller.AI.StartPath();
        }
        controller.AI.UpdatePath();
    }    
}
