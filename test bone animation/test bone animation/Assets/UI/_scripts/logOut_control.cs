using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logOut_control : MonoBehaviour {

    public GameObject logout_panel;
	public GameObject character;
	playerbehave player;
	// Use this for initialization
	void Start () {
        logout_panel.SetActive(false);
		player = character.GetComponent<playerbehave> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void open() {
		StartCoroutine (offline());
    }
	IEnumerator offline(){
		WWWForm form = new WWWForm ();
		form.AddField ("differ", "logout");
		form.AddField ("account", PlayerAccount.ACCOUNT);
		form.AddField ("hp", player.HP.ToString());
		form.AddField ("exp", player.Exp.ToString());
		WWW www = new WWW ("http://140.136.150.77/updata1.php", form);
		yield return www;

		logout_panel.SetActive(true);
	}

}
