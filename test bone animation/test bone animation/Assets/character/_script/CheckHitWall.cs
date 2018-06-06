using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitWall : MonoBehaviour {

	playerbehave script;

	void Start () {
		script = gameObject.transform.parent.GetComponent<playerbehave> ();
	}
		
	void Update () {

	}
	void OnTriggerEnter2D (Collider2D other){
		if (other.gameObject.CompareTag ("Ground")) {
			script.hitwall = true;
		}
	}
	void OnTriggerExit2D (Collider2D other){
		if (other.gameObject.CompareTag ("Ground"))
			script.hitwall = false;
	}
}
