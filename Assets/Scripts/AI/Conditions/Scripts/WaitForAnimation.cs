using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ( menuName = "AI/Conditions/WaitForAnimation")]
public class WaitForAnimation : AICondition {

    public override bool Decide(StateController controller)
    {
        if (controller.currentState.actions[0])
        {
            return false;
        }
        else
        {
            return true;
        }
    }    
}
