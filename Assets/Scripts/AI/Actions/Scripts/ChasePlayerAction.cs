using System;
using System.Collections;
using System.Collections.Generic;
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
        if(controller.AI.path == null)
        {
            controller.AI.StartPath();
        }
        controller.AI.UpdatePath();
    }    
}
