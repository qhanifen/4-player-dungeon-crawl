using UnityEngine;

[CreateAssetMenu ( menuName = "AI/Conditions/Wait For Timer")]
public class WaitForTimer : AICondition {

    public float waitTime;

    public override bool Decide(StateController controller)
    {
        return controller.CheckIfCountDownElapsed(waitTime);
    }    
}
