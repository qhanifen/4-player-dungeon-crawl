using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

    public bool aiActive;
    public AIBehavior AI;
    
    public AIState currentState;
    public AIState remainState;
    public AIState previousState;
    public AITransition[] anyStates;

    [HideInInspector] public float stateTimeElapsed;
    
    //Call in AIBehavior to initialize StateMachine
    public void Initialize(AIBehavior aiAgent)
    {
        AI = aiAgent;
        //aiActive = true;
	}
	
	// Update called through AI Update Machine Event
	public void UpdateController()
    {
        if (aiActive)
        {
            currentState.UpdateState(this);
            for(int i = 0; i< anyStates.Length; i++)
            {
                if(anyStates[i].condition.Decide(this) == true)
                {
                    ToNextState(anyStates[i].trueState);
                }
            }
        }
	}

    public void ToNextState(AIState nextState)
    {
        if (nextState != remainState)
        {
            currentState.OnStateExit(this);
            previousState = currentState;
            currentState = nextState;
            nextState.OnStateEnter(this);
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float timer)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= timer);        
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }

    void OnDrawGizmos()
    {
        if(currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, 1.0f);
        }
    }
}
