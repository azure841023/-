using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateDoor : MonoBehaviour {

	public Transform nextPosition;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "Player") {
			other.SendMessage ("ApplyTranslate_bool", true);
			other.SendMessage ("ApplyTranslate_pos", nextPosition);
		}
	}
	void OnTriggerExit2D (Collider2D other){
		if (other.tag == "Player") {
			other.SendMessage ("ApplyTranslate_bool", false);
		}
	}
}
