using UnityEngine;
using System.Collections.Generic;
using Rewired;

public class PlayerManager : MonoSingletonPersistent<PlayerManager> 
{
    public HeroesList heroesList;
    public List<PlayerEntity> players;

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
        players[playerID].heroID = heroID;
    }

    public Vector3[] GetHeroPositions()
    {
        Vector3[] playerPositions = new Vector3[players.Count];
        for(int i=0; i< playerPositions.Length; i++)
        {
            if(players[i] != null)
            {
                playerPositions[i] = GameManager.instance.heroes[i].transform.position;
            }            
        }
        return playerPositions;
    }   

    private void AssignPlayers()
    {
        IList<Player> players = ReInput.players.GetPlayers();        
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i].name);
            this.players[i].playerID = i;    
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
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].playerID == playerID)
                    {
                        players[i].playerController.active = toggle;

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
