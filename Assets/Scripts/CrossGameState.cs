using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CrossGameState : MonoSingletonPersistent<CrossGameState> {
        
    private const string loadingScene = "Loading Scene";
    private const string menuScene = "Main Menu";

    public PlayerManager playerManager;
    public GameManager gameManager;

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    // Use this for initialization
    IEnumerator StartGame ()
    {
        LoadSceneAsync(menuScene);
        yield return new InitializeSystem(playerManager);
        yield return new InitializeSystem(gameManager);
	}
	
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

	public void LoadSceneAsync(string sceneName)
    {
        //SceneManager.LoadScene(loadingScene);
        SceneManager.LoadSceneAsync(sceneName);
    }
}
