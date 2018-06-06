using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerInfo : MonoBehaviour {

	public Text txt;
	string playername;
	int playerlv;

	void Start () {
		
	}

	void Update () {
		
	}

	void Applyname(string name){
		playername = name;
	}
	void Applylv(int level){
		playerlv = level;
		changeInfo ();
	}
	void changeInfo(){
		txt.text = playername + "\nLV." + playerlv;
	}
}
