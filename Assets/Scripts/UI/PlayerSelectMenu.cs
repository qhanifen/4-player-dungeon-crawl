using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectMenu : MonoBehaviour {

    #region Player Variables
    public int ID;
    public PlayerEntity playerEntity;
    public bool playerReady;
    #endregion

    #region UI Variables
    public MainMenu menu;
    public Text heroNameText;
    public static string[] heroNames = { "Swordsman", "Mage", "Archer", "Fighter" };
    #endregion

    #region Hero Model Variables
    public int heroSelectionID;
    public GameObject[] heroModels;    
    #endregion

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

    public void SwitchHero(int dir)
    {
        heroModels[heroSelectionID].SetActive(false);
        heroSelectionID += dir;
        if(heroSelectionID == heroModels.Length)
        {
            heroSelectionID = 0;
        }
        else if(heroSelectionID < 0)
        {
            heroSelectionID = heroModels.Length - 1;
        }
        heroModels[heroSelectionID].SetActive(true);
        heroNameText.text = heroNames[heroSelectionID];
    }    

    public void TogglePlayerReady()
    {
        playerReady = !playerReady;
        MainMenu.playerReady[ID] = playerReady;
    }
}
