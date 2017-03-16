using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICondition : ScriptableObject
{
    public abstract bool Decide(StateController controller); 	
}
