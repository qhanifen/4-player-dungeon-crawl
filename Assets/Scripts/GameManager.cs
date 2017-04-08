using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameManager : MonoSingletonPersistent<GameManager>
{
    public List<PlayerController> playerControllers;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            playerControllers.Add(new PlayerController(i));
        }
        ReInput.ControllerConnectedEvent += ReInput_ControllerConnectedEvent;
        ReInput.ControllerDisconnectedEvent += ReInput_ControllerDisconnectedEvent;
    }

    private void ReInput_ControllerConnectedEvent(ControllerStatusChangedEventArgs obj)
    {
        playerControllers[obj.controllerId].active = true;
        Debug.Log("Player " + obj.controllerId + " connected");
    }

    private void ReInput_ControllerDisconnectedEvent(ControllerStatusChangedEventArgs obj)
    {
        playerControllers[obj.controllerId].active = false;
        Debug.Log("Player " + obj.controllerId + " disconnected");
    }
}
