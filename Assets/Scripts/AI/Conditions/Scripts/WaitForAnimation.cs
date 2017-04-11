using UnityEngine;

[CreateAssetMenu ( menuName = "AI/Conditions/WaitForAnimation")]
public class WaitForAnimation : AICondition {

    public float waitTime;

    public override bool Decide(StateController controller)
    {
        return controller.CheckIfCounDownElapsed(waitTime);
    }    
}
