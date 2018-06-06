using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logOut_cancel : MonoBehaviour {

    public GameObject logout_panel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void cancel_logout() {
        logout_panel.SetActive(false);
    }
}
