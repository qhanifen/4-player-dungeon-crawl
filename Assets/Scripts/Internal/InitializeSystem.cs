using System.Collections;
using UnityEngine;

public class InitializeSystem : CustomYieldInstruction {

    ISystem m_System;

    public override bool keepWaiting
    {
        get
        {
            return !m_System.Initialized;
        }
    }    

    public InitializeSystem(ISystem system)
    {
        MonoBehaviour mono = system as MonoBehaviour;
        if (mono != null)
        {
            GameObject obj = UnityEngine.Object.Instantiate(mono.gameObject);            
            m_System = system;
            m_System.Initialize();
        }
        else
        {
            Debug.Log("Error with initializing system.");
        }
    }
}
