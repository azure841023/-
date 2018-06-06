using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class create_changeThing : MonoBehaviour {

	public GameObject change;
	public GameObject target;
	Anima2D.SpriteMeshAnimation script;
	private GameObject[] picture;
	private int curNumber;			//當前編號
	private int maxNumber;			//編號最大值

	void Start () {
		int num;
		num = change.transform.childCount;
		picture = new GameObject[num];
		script = target.GetComponent<Anima2D.SpriteMeshAnimation> ();

		maxNumber = num;

		for (int i = 0; i < num; i++) {
			picture [i] = change.transform.GetChild (i).gameObject;
			picture [i].SetActive (false);
		}

		curNumber = script.frame;
		picture [curNumber].SetActive (true);
	}
	
	public void right(){
		picture [curNumber].SetActive (false);
		curNumber++;
		if (curNumber >= maxNumber)
			curNumber = 0;
		picture [curNumber].SetActive (true);
		script.frame = curNumber;
	}

	public void left(){
		picture [curNumber].SetActive (false);
		curNumber--;
		if (curNumber < 0)
			curNumber = maxNumber - 1;
		picture [curNumber].SetActive (true);
		script.frame = curNumber;
	}
}
