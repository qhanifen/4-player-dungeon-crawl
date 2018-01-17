using Rewired;
using UnityEngine;

public class PlayerSelectMenu : MonoBehaviour {

    public int ID;
    public PlayerEntity playerEntity;
    public bool playerReady;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (playerEntity != null && playerEntity.active)
        {
            GetPlayerInput();
        }
    }


    public void AssignPlayer(PlayerEntity assignedPlayer)
    {
        playerEntity = assignedPlayer;
    }

    void GetPlayerInput()
    {
        float leftAxisX = playerEntity.rewiredPlayer.GetAxis("Move Horizontal");
        float leftAxisY = playerEntity.rewiredPlayer.GetAxis("Move Vertical");
    }
    
}
