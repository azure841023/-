using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOnGround : MonoBehaviour {


	playerbehave script;		//上層character的playbehave程式
	private Animator animator;


	void Start () {
		script = gameObject.transform.parent.GetComponent<playerbehave> ();
		//animator = gameObject.transform.parent.GetComponent<Animator> ();
	}
	

	void Update () {

	}
	void OnTriggerStay2D (Collider2D other){
		if (other.gameObject.CompareTag ("Ground")) {
			script.onground = true;
			//animator.SetBool ("OnGround", true);
		}
	}
	void OnTriggerExit2D (Collider2D other){
		if (other.gameObject.CompareTag ("Ground")) {
			script.onground = false;
			//animator.SetBool ("OnGround", false);
		}
	}
}
