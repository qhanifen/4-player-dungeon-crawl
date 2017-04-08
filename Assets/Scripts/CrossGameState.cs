using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CrossGameState : MonoSingletonPersistent<CrossGameState> {

    private const string k_startGameScene = "Main Menu";
    private const string k_loadingSceneName = "Loading Scene";

	// Use this for initialization
	void StartGame () {
        LoadScene(k_startGameScene);
	}
	
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

	public void LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadScene(k_loadingSceneName);
        SceneManager.LoadSceneAsync(sceneName);
    }
}
