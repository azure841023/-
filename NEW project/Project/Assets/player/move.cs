using UnityEngine;
//using UnityEngine.UI;
using System.Collections;

public class move : MonoBehaviour {

	public Rigidbody2D rb;
	public bool onGround;
	public float high = 100.0f;
	public float speed = 0;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector2 right =new Vector2(speed,0);
		Vector2 left = new Vector2 (-speed, 0);
		float moveX = rb.velocity.x;
		float moveY = rb.velocity.y;
		Vector2 movement = new Vector2 (moveX, moveY);
		if (Input.GetKey("right")) {
			rb.transform.rotation = Quaternion.Euler (0, 0, 0);
			rb.velocity = new Vector2 (speed, rb.velocity.y);
		}

		if (Input.GetKey("left")){
			rb.transform.rotation = Quaternion.Euler (0, 180, 0);
			rb.velocity = new Vector2 (-speed, rb.velocity.y);
		}
		if (Input.GetKeyUp ("right") || Input.GetKeyUp("left")) {
			rb.velocity = new Vector2 (0, rb.velocity.y);
		}
		Vector2 up = new Vector2 (0, high);
		if (Input.GetKey ("up") && onGround == true) {
			rb.velocity = movement + up;
		}
	}

	void OnCollisionEnter2D (Collision2D other){
		if (other.gameObject.CompareTag ("ground"))
		onGround = true;
	}
	void OnCollisionExit2D (Collision2D other){
		if (other.gameObject.CompareTag ("ground")) {
			onGround = false;
		}
	}
}
