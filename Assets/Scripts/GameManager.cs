using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingletonPersistent<GameManager>
{
    public HeroesList heroesList;
    public List<Hero> heroes;

    public enum GameState
    {
        Menu,
        Loading,
        Play,
        GameOver
    }
    public GameState gameState;


    private void Start()
    {
        gameState = GameState.Menu;
    }

    public void SpawnHeroes(Transform spawnPoint)
    {
        foreach(PlayerEntity player in PlayerManager.instance.players)
        {
            Hero hero = Instantiate(heroesList.heroes[player.heroID], Vector3.zero, Quaternion.identity);
            heroes.Add(hero);
            Camera.main.GetComponent<CameraController>().trackedObjects.Add(hero.gameObject);            
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
