using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : AICondition
{
    public override bool Decide(StateController controller)
    {        
        if (FoundEnemy(controller) || Attacked(controller))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //ToDo: Work on clean way to subscribe to an OnAttacked Event in AIBehavior 
    private bool Attacked(StateController controller)
    {
        if (controller.AI.alerted)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool FoundEnemy(StateController controller)
    {
        RaycastHit hit;
        if (Physics.SphereCast(controller.AI.transform.position, 3.0f, controller.AI.transform.forward, out hit, 20.0f, controller.AI.stats.attackLayerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
