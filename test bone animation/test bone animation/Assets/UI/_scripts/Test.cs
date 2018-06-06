using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour {

	// Use this for initialization
	public void OnStartGame(string SceneName)
    {
       //Application.LoadLevel(SceneName);
		SceneManager.LoadScene(SceneName);//讀取場景,場景名稱
    }

}
