using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class logOut : MonoBehaviour {

	void Start () {

	}
	

	void Update () {
		
	}

    public void logout() {
		SceneManager.LoadScene ("01 Start Scene");
    }
}
