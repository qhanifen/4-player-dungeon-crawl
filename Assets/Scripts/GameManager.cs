using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameManager : MonoSingletonPersistent<GameManager>
{
    public List<PlayerController> playerControllers;
    public List<Hero> heroes;

    private void Start()
    {        
        ReInput.ControllerConnectedEvent += ReInput_ControllerConnectedEvent;
        ReInput.ControllerDisconnectedEvent += ReInput_ControllerDisconnectedEvent;
    }

    public void SpawnHeroes(Transform spawnPoint)
    {
        int spawnCount = playerControllers.Count;
        foreach (PlayerController player in playerControllers)
        {
            //heroes.Add(Instantiate(player.hero, spawnpoint, Quaternion.identity))
        }
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
