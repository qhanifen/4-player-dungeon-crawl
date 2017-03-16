using UnityEngine;
using System.Collections.Generic;
using Rewired;

public class PlayerManager : MonoSingletonPersistent<PlayerManager> 
{
	public List<Hero> heroes;
	public IList<Player> players;

	public enum ControlType
	{
		InMenu,
		InGame
	}

    void Start()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
        ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;
        AssignPlayers();
	}

    public Vector3[] GetHeroPositions()
    {
        Vector3[] playerPositions = new Vector3[heroes.Count];
        for(int i=0; i< playerPositions.Length; i++)
        {
            if(heroes[i] != null)
            {
                playerPositions[i] = heroes[i].transform.position;
            }            
        }
        return playerPositions;
    }

    public static Hero GetHero(int index)
    {
        return instance.heroes[index];
    }

    private void AssignPlayers()
    {
        players = ReInput.players.GetPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i].name);
            heroes[i].GetComponent<PlayerController>().player = players[i];
        }
    }

	#region ReInput Events
	void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
        AssignPlayers();

    }

	void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{

	}

	void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args)
	{
		
	}
	#endregion

    
}
