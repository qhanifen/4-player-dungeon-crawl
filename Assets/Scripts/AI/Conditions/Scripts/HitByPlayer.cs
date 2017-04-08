using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitByPlayer : AICondition
{
    public override bool Decide(StateController controller)
    {
        return false;
    }

    private void TookDamage()
    {
    }	
}
