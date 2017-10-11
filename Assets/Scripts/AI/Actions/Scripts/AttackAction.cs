using UnityEngine;

[CreateAssetMenu( menuName = "AI/Actions/Attack Action")]
public class AttackAction : AIAction
{

    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    private void Attack(StateController controller)
    {
        if(controller.AI is Enemy)
        {
            controller.AI.Attack();
        }
    }
}
