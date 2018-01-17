using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingletonPersistent<GameManager>, ISystem
{
    public HeroesList heroesList;
    public List<Hero> heroes;

    public bool Initialized { get { return m_initialized; } set { m_initialized = value; } }
    bool m_initialized = false;

    public enum GameState
    {
        Menu,
        Loading,
        Play,
        GameOver
    }
    public GameState gameState;
    
    //Encapsulate any starting methods here
    public void Initialize()
    {
        gameState = GameState.Menu;
        Initialized = true;        
        Debug.Log("GameManager initialized.");
    }

    public void SpawnHeroes(Transform spawnPoint)
    {
        foreach(PlayerEntity player in PlayerManager.instance.players)
        {
            PlayerController controller = Instantiate(player.playerController, LevelManager.instance.spawnPosition.position, Quaternion.identity);
            player.playerController = controller;
            Camera.main.GetComponent<CameraController>().trackedObjects.Add(controller.transform);            
        }
    }

    public Transform GetClosestHero(Transform pos)
    {
        int heroID = 0;
        float dist = 0f;
        for (int i = 0; i < heroes.Count; i++)
        {
            Transform hero = heroes[i].transform;
            float calcDist = (hero.position - pos.position).sqrMagnitude;
            if (dist == 0f)
            {
                dist = calcDist;
                heroID = i;
                break;
            }
            if (calcDist < dist)
            {
                heroID = i;
            }
        }
        return heroes[heroID].transform;
    }

    public static Hero GetHero(int index)
    {
        return instance.heroes[index];
    }
}
