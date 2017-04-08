using System;

[Serializable]
public class AITransition 
{
    public AICondition condition;
    public AIState trueState;
    public AIState falseState;
}
