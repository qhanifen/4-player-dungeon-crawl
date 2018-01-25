using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    IList<Player> players;
    public List<PlayerSelectMenu> playerMenus;
    public static bool[] playerReady = { false, false, false, false };
    
    public enum MenuState
    {
        Title,
        PlayerSelect,
    }

    public MenuState menuState;

	void Start()
    {
        players = ReInput.players.GetPlayers();
        menuState = MenuState.Title;
        for (int i = 0; i < players.Count; i++)
        {
            //playerMenus[i]. = 
        }
    }

    void Update()
    {
        switch(menuState)
        {
            case MenuState.Title:
                WaitForPlayerStart();
                break;
            case MenuState.PlayerSelect:
                PollForPlayerInput();
                break;
            default:
                break;
        }
    }

    void WaitForPlayerStart()
    {
        foreach(Player player in players)
        {
            if(player.GetAnyButtonDown())
            {
                PresentMenu(MenuState.PlayerSelect);
            }
        }
    }

    void PollForPlayerInput()
    {
        foreach(Player player in players)
        {

        }
    }

    void NavigateMenu(Player player, Vector3 dir)
    {

    }

    void PresentMenu(MenuState menu)
    {
        menuState = menu;
        Debug.Log(menuState);
    }

}
