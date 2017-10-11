using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : MonoSingleton<CharacterLoader> {

    public HeroesList heroesList;

    public List<GameObject> characterLoaders;
    
    void Start()
    {
        
    }

    void EnableCharacterLoader(int playerID)
    {
        characterLoaders[playerID].SetActive(true);
    }

    void LoadCharacter(int playerID, int characterID, int characterColorID)
    {

    }
}
