using UnityEngine;

[CreateAssetMenu(menuName = "AI/Conditions/AI Death")]
public class DeathCondition : AICondition
{
    public override bool Decide(StateController controller)
    {
        return CheckHealthIsZero(controller);
    }

    private bool CheckHealthIsZero(StateController controller)
    {
        if(controller.AI.currentHealth <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
