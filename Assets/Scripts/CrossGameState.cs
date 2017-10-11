using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CrossGameState : MonoSingletonPersistent<CrossGameState> {

    private const string startGameScene = "Main Menu";
    private const string loadingSceneName = "Loading Scene";

	// Use this for initialization
	void StartGame () {
        LoadScene(startGameScene);
	}
	
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

	public void LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadScene(loadingSceneName);
        SceneManager.LoadSceneAsync(sceneName);
    }
}
