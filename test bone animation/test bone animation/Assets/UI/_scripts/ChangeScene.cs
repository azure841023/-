using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	public void Scene(string SceneName)
	{
		//Application.(SceneName);
		SceneManager.LoadScene(SceneName);//讀取場景,場景名稱
	}
}
