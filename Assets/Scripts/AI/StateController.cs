using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

    public bool aiActive;
    public AIBehavior AI;

    public Dictionary<string, AIState> states;
    public AIState currentState;
    
    //Call in AIBehavior to initialize StateMachine
    public void Initialize(AIBehavior aiAgent)
    {
        AI = aiAgent;       
	}
	
	// Update called through AI Update Machine Event
	public void Update()
    {
        if (aiActive)
        {
            currentState.UpdateState(this);
        }
	}

    public void ToNextState(AIState nextState)
    {
        currentState = nextState;
    }

    void OnDrawGizmos()
    {
        if(currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, 5.0f);
        }
    }
}
