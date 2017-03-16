using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LoadSceneOnClick : MonoBehaviour
{
    public void LoadScene1()  {
		SceneManager.LoadScene ("magic_projectiles");
	}
    public void LoadScene2()  {
        SceneManager.LoadScene("magic_sprays");
	}
    public void LoadScene3()  {
        SceneManager.LoadScene("magic_aura");
	}
    public void LoadScene4()  {
		SceneManager.LoadScene ("magic_modular");
	}
    public void LoadScene5()  {
        SceneManager.LoadScene ("magic_domes");
	}
    public void LoadScene6()  {
        SceneManager.LoadScene ("magic_shields");
	}
    public void LoadScene7()  {
        SceneManager.LoadScene ("magic_sphereblast");
	}
    public void LoadScene8()  {
        SceneManager.LoadScene ("magic_enchant");
    }
    public void LoadScene9()  {
        SceneManager.LoadScene ("magic_slash");
    }
    public void LoadScene10() {
        SceneManager.LoadScene("magic_charge");
    }
}