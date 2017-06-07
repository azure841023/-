using UnityEngine;
using System.Collections;

public class animatorcontrol : MonoBehaviour {
	private Animator animator;
	private bool move;
	private bool jump;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		move = false;
		jump = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey ("right") || Input.GetKey ("left"))
			move = true;
		else
			move = false;
		animator.SetBool ("move", move);
		animator.SetBool ("jump", jump);
	}
	void OnTriggerEnter2D (Collider2D other){
		if (other.gameObject.CompareTag ("ground"))
			jump = false;
	}
	void OnTriggerExit2D (Collider2D other){
		if (other.gameObject.CompareTag ("ground")) {
			jump = true;
		}
	}
}
