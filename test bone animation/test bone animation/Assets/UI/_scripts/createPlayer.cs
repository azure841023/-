using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class createPlayer : MonoBehaviour {

	public InputField inputName;
	public GameObject head, body, weapon;
	Anima2D.SpriteMeshAnimation h_script, b_script, w_script;
	char[] deleteChars = {' ', ',', '=', ':', '\t' };	//欲刪除之符號集合

	void Start(){
		h_script = head.transform.GetComponent<Anima2D.SpriteMeshAnimation> ();
		b_script = body.transform.GetComponent<Anima2D.SpriteMeshAnimation> ();
		w_script = weapon.transform.GetComponent<Anima2D.SpriteMeshAnimation> ();
	}

	public void OnClick(){
		string[] playername = inputName.text.Split (deleteChars);
		if (playername[0] != null)
			StartCoroutine (create ("na", PlayerAccount.ACCOUNT, playername[0], h_script.frame, b_script.frame, w_script.frame));
	}

	IEnumerator create(string opcode, string account, string username, int head, int body, int weapon){
		
		//傳送username作為php中的account,name,head編號,body編號,weapon編號，新增角色
		WWWForm table = new WWWForm();
		table.AddField("differ", opcode);
		table.AddField("account", account);
		table.AddField("name", username);
		table.AddField ("head", head);
		table.AddField ("body", body);
		table.AddField ("weapon", weapon);
		WWW wwwt = new WWW("http://140.136.150.77/start2.php", table);

		yield return wwwt;
		Debug.Log (wwwt.text);

		//Application.LoadLevel(nextScene);
		SceneManager.LoadScene("try");//讀取場景,場景名稱
	}
}
