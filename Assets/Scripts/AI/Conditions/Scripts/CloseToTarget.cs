using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Conditions/Close to Target")]
public class CloseToTarget : AICondition {

    public override bool Decide(StateController controller)
    {
        return CheckDistance(controller);
    }

    private bool CheckDistance(StateController controller)
    {
        if(controller.AI.target == null)
        {
            return false;
        }
        if (Vector3.SqrMagnitude(controller.AI.target.position - controller.AI.transform.position) <= Mathf.Pow(controller.AI.attackRange, 2))
        {
            return true;
        }
        else
        {
            return false;
        }        
    }	
}
