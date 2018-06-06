using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehave : MonoBehaviour {

	public Transform player;
	public Transform edge_left;
	public Transform edge_right;
	float edge_distance = 20f;

	void Start () {
		
	}

	void Update () {
		gameObject.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, -10f);
		if (gameObject.transform.position.x < edge_left.position.x + edge_distance)
			gameObject.transform.position = new Vector3 (edge_left.position.x + edge_distance, player.transform.position.y, -10f);
		else if (gameObject.transform.position.x > edge_right.position.x - edge_distance)
			gameObject.transform.position = new Vector3 (edge_right.position.x - edge_distance, player.transform.position.y, -10f);
		/*if (player.transform.position.x > -8 && player.transform.position.x < 230)
			gameObject.transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, -10f);
		else
			gameObject.transform.position = new Vector3 (gameObject.transform.position.x, player.transform.position.y, -10f);*/
	}
}
