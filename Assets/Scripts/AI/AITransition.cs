using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Transition")]
public class AITransition : ScriptableObject
{
    public AICondition condition;
    public AIState trueState;
    public AIState falseState;	
}
