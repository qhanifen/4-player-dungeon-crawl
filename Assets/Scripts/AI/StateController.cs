﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

    public bool aiActive;
    public AIBehavior AI;

    public Dictionary<string, AIState> states;
    public AIState currentState;
    public AIState remainState;
    public AIState previousState;
    public AIState[] anyStates;

    [HideInInspector] public float stateTimeElapsed;
    
    //Call in AIBehavior to initialize StateMachine
    public void Initialize(AIBehavior aiAgent)
    {
        AI = aiAgent;
        //aiActive = true;
	}
	
	// Update called through AI Update Machine Event
	public void Update()
    {
        if (aiActive)
        {
            currentState.UpdateState(this);
            for(int i = 0; i< anyStates.Length; i++)
            {
                anyStates[i].UpdateState(this);
            }
        }
	}

    public void ToNextState(AIState nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
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
