using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingletonPersistent<GameManager>
{
    public List<PlayerController> playerControllers;
    public List<Hero> heroes;

    private void Start()
    {       
        
    }

    public void SpawnHeroes(Transform spawnPoint)
    {
        int spawnCount = playerControllers.Count;
        foreach (PlayerController player in playerControllers)
        {
            //heroes.Add(Instantiate(player.hero, spawnpoint, Quaternion.identity))
        }
    }
}
