using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicControl : MonoBehaviour {

	[SerializeField]
	private GameObject[] music;
	int num;
	public Transform character;
	// Use this for initialization
	void Start () {
		num = gameObject.transform.childCount;
		music = new GameObject[num];

		for (int i = 0; i < num; i++) {
			music [i] = gameObject.transform.GetChild (i).gameObject;
			music [i].SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (character.transform.position.y >= 180f) {
			if (music [0].activeSelf == false) {
				music [0].SetActive (true);
				music [1].SetActive (false);
				music [2].SetActive (false);
			}
		} else if (character.transform.position.y <= -120f) {
			if (music [2].activeSelf == false) {
				music [0].SetActive (false);
				music [1].SetActive (false);
				music [2].SetActive (true);
			}
		} else {
			if (music [1].activeSelf == false) {
				music [0].SetActive (false);
				music [1].SetActive (true);
				music [2].SetActive (false);
			}
		}
	}
}
