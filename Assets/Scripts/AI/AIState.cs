using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI State")]
public class AIState : ScriptableObject {

    public AIAction[] actions;
    public AITransition[] transitions;

    public Color sceneGizmoColor = Color.grey;

    public void UpdateState(StateController controller)
    {
        for(int i=0; i<actions.Length; i++)
        {
            actions[i].Act(controller);
        }
        CheckTransitionStates(controller);
    }

    private void CheckTransitionStates(StateController controller)
    {
        for(int i=0; i<transitions.Length; i++)
        {
            if(transitions[i].condition == true)
            {

            }
        }
    }   
}
