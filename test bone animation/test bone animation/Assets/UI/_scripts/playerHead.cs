using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHead : MonoBehaviour {

	private GameObject[] picture;

	// Use this for initialization
	void Start () {
		int num;
		num = gameObject.transform.childCount;
		picture = new GameObject[num];

		for (int i = 0; i < num; i++) {
			picture [i] = gameObject.transform.GetChild (i).gameObject;
			picture [i].SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void Applyhead(int head_num){
		picture [head_num].SetActive (true);
	}
}
