using UnityEngine;
using System.Collections.Generic;
using Rewired;

public class PlayerManager : MonoSingletonPersistent<PlayerManager> 
{
    public HeroesList heroesList;
    public List<PlayerController> playerControllers;

	public enum ControlType
	{
		InMenu,
		InGame
	}

    void Start()
    {
        //playerControllers = new List<PlayerController>();
        ReInput.ControllerConnectedEvent += ReInput_ControllerConnectedEvent;
        ReInput.ControllerDisconnectedEvent += ReInput_ControllerDisconnectedEvent;
        AssignPlayers();
	}

    public void SetPlayerHero(int playerID, int heroID)
    {
        playerControllers[playerID].hero = heroesList.heroes[heroID];
    }

    public Vector3[] GetHeroPositions()
    {
        Vector3[] playerPositions = new Vector3[playerControllers.Count];
        for(int i=0; i< playerPositions.Length; i++)
        {
            if(playerControllers[i] != null)
            {
                playerPositions[i] = playerControllers[i].hero.transform.position;
            }            
        }
        return playerPositions;
    }    

    public static Hero GetHero(int index)
    {
        return instance.playerControllers[index].hero;
    }

    private void AssignPlayers()
    {
        IList<Player> players = ReInput.players.GetPlayers();        
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i].name);
            playerControllers[i].player = players[i];
        }
    }

    private void ToggleControllerActive(ControllerStatusChangedEventArgs obj, bool toggle)
    {
        int controllerID = obj.controllerId;
        foreach (Player player in ReInput.players.GetPlayers())
        {
            int playerID = player.id;
            if (player.controllers.ContainsController(obj.controllerType, obj.controllerId))
            {
                for (int i = 0; i < playerControllers.Count; i++)
                {
                    if (playerControllers[i].playerID == playerID)
                    {
                        playerControllers[i].active = toggle;

                        string connecitonStatus = toggle ? "connected" : "disconnected";
                        Debug.Log("Player " + obj.controllerId + " " + connecitonStatus);
                        return;
                    }
                }
            }
        }
    }

	#region ReInput Events
    private void ReInput_ControllerConnectedEvent(ControllerStatusChangedEventArgs obj)
    {
        ToggleControllerActive(obj, true);
    }

    private void ReInput_ControllerDisconnectedEvent(ControllerStatusChangedEventArgs obj)
    {
        ToggleControllerActive(obj, false);
    }
    #endregion


}
