using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class relife : MonoBehaviour {

	public GameObject character;
	playerbehave player;
	public GameObject picture;
	public GameObject rip;
	public GameObject die_panel;
	// Use this for initialization
	void Start () {
		player = character.GetComponent<playerbehave> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void revive(){
		player.HP = player.maxHP;
		player.death = false;
		player.Exp = player.Exp * 0.9f;
		character.transform.position = new Vector3(10f,210f,character.transform.position.z);
		die_panel.SetActive(false);
		picture.SetActive(true);
		rip.SetActive(false);
	}
}
