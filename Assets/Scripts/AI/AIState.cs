using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI State")]
public class AIState : ScriptableObject {

    public AIAction[] enterStateActions;
    public AIAction[] updateStateActions;
    public AIAction[] exitStateActions;
    public AITransition[] transitions;

    public Color sceneGizmoColor = Color.grey;

    public void OnStateEnter(StateController controller)
    {
        for (int i = 0; i < enterStateActions.Length; i++)
        {
            enterStateActions[i].Act(controller);
        }
    }

    public void UpdateState(StateController controller)
    {
        for(int i=0; i<updateStateActions.Length; i++)
        {
            updateStateActions[i].Act(controller);
        }
        CheckTransitionStates(controller);
    }

    public void OnStateExit(StateController controller)
    {
        for (int i = 0; i < exitStateActions.Length; i++)
        {
            exitStateActions[i].Act(controller);
        }
    }

    private void CheckTransitionStates(StateController controller)
    {
        for(int i=0; i<transitions.Length; i++)
        {
            if (transitions[i].condition.Decide(controller) == true)
            {
                controller.ToNextState(transitions[i].trueState);
            }
        }
    }       
}
