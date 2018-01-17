using UnityEngine;
using System.Collections.Generic;
using Rewired;

public class PlayerManager : MonoSingletonPersistent<PlayerManager>, ISystem
{
    public HeroesList heroesList;
    public List<PlayerEntity> players;

    public bool Initialized { get { return m_initialized; } set { m_initialized = value; } }
    bool m_initialized = false;
    
    ////Encapsulate any starting methods here
    public void Initialize()
    {        
        ReInput.ControllerConnectedEvent += ReInput_ControllerConnectedEvent;
        ReInput.ControllerDisconnectedEvent += ReInput_ControllerDisconnectedEvent;
        players = new List<PlayerEntity>();
        AssignPlayers();
        Initialized = true;
        Debug.Log("PlayerManager initialized.");
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
        IList<Player> playerList = ReInput.players.GetPlayers();        
        for (int i = 0; i < playerList.Count; i++)
        {
            Debug.Log(playerList[i].name);
            PlayerEntity player = new PlayerEntity(i, playerList[i]);
            players.Add(player);
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
