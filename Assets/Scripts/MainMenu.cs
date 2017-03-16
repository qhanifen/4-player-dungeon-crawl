using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    IList<Player> players;

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
    }

    void Update()
    {
        switch(menuState)
        {
            case MenuState.Title:
                WaitForPlayerStart();
                break;
            case MenuState.PlayerSelect:
            default:
                break;
        }
    }

    void WaitForPlayerStart()
    {
        foreach(Player player in players)
        {
            if(player.GetButtonDown("Start"))
            {
                PresentMenu(MenuState.PlayerSelect);
            }
        }
    }

    void PresentMenu(MenuState menu)
    {
        menuState = menu;
        Debug.Log(menuState);
    }

}
