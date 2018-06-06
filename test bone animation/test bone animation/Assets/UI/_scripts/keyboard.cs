using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keyboard : MonoBehaviour
{
	public InputField name,tmp;
	public Text txt;
	public int linenumber;
	[HideInInspector]
	public static List<string> line = new List<string>();						//聊天室訊息

	GameObject target;		//character物件
	playerbehave script;	//上者裡面的playerbehave程式

	void Start()
	{
		linenumber = 9;

		target = GameObject.Find("character");
		script = target.GetComponent<playerbehave> ();
	}

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.Return) && name.text!="") {

			txt.text += PlayerAccount.ACCOUNT + ":" + name.text + "\n";
			line.Add (PlayerAccount.ACCOUNT + ":"+ name.text + "\n");
			script.Send ("S,"+ PlayerAccount.ACCOUNT +"," + name.text );		//藉由character的playbehave程式傳輸輸入字到server
			name.text = "";
			if (line.Count > linenumber) {										//超過行數 (聊天室顯示行數) 時的調整
				txt.text = "";
				for (int i = 0; i < linenumber; i++) {
					line [i] = line [i + 1];
					txt.text += line [i];
				}
				line.RemoveAt (linenumber);
			}
		//	Debug.Log ("line.Count=" + line.Count);
		}
	}
}
