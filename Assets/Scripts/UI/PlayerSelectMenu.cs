using Rewired;
using UnityEngine;

public class PlayerSelectMenu : MonoBehaviour {

    public int ID;
    public Player player;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (player != null)
        {
            GetPlayerInput();
        }
    }


    public void AssignPlayer(Player assignedPlayer)
    {
        player = assignedPlayer;
    }

    void GetPlayerInput()
    {
        float leftAxisX = player.GetAxis("Move Horizontal");
        float leftAxisY = player.GetAxis("Move Vertical");
    }
    
}
