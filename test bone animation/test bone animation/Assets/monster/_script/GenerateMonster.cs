using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMonster : MonoBehaviour {

	public GameObject[] OtherPlayer = new GameObject[4];		//存4種怪物種類
	[Header("出現位置")]
	public Vector2 target;
	int m_id;				//怪物ID
	int m_alive;			//怪物是否存活
	int m_type;				//怪物種類
	Vector3 position;		//出現位置

	char[] deleteChars = { ' ', ',', '=', ':', '\t' };			//欲刪除之符號集合

	void Start () {
		StartCoroutine (GetInfo ("sm"));						//執行php中的 sm (select monster) 
	}

	void Update () {
		//position = new Vector3(target.x, target.y, 0);			//將出現位置position持續更新成目標位置target
	}
	public void Summon(){										//外部呼叫，用於產生怪物

		var monster = Instantiate (OtherPlayer[m_type], position, Quaternion.identity);
		monster.SendMessage ("ApplyID", m_id);					//將怪物的id傳送給新生的怪物並交由其保存
		monster.SendMessage("ApplyBorn",gameObject.GetComponent<GenerateMonster>());
	}

	IEnumerator GetInfo(string opcode){

		WWWForm form = new WWWForm ();
//		Debug.Log (PlayerAccount.usernum);
		form.AddField ("differ", opcode);

		WWW www = new WWW ("http://140.136.150.77/monster.php", form);
		yield return www;

//		Debug.Log (www.text);

		//www字串切割
		string[] info = www.text.Split (deleteChars);		//切割完依序是id, 存活bool, 種類int, 出現位置
		//Debug.Log (info.Length);
		for (int i = 0; i < (info.Length - 1) / 6; i++) {
			m_id = int.Parse (info [6 * i]);
			m_alive = int.Parse (info [6 * i + 1]);
			m_type = int.Parse (info [6 * i + 2]);
			//target.x = float.Parse (info [6 * i + 3]);
			//target.y = float.Parse (info [6 * i + 4]);
			position = new Vector3(float.Parse (info [6 * i + 3]), float.Parse (info [6 * i + 4]), 0);
//			Debug.Log (position);
			if (m_alive == 1) {
				Summon ();
			}
		}
	}

	public void ApplyNew(int id){
		StartCoroutine (NewBorn (id));
	}
	IEnumerator NewBorn(int num){
		WWWForm form = new WWWForm ();
		form.AddField ("differ", "nm");
		form.AddField ("id", num);
		WWW www = new WWW ("http://140.136.150.77/monster.php", form);
		yield return www;
		string[] info = www.text.Split (deleteChars);
		m_id = int.Parse (info [0]);
		m_alive = int.Parse (info [1]);
		m_type = int.Parse (info [2]);
		target.x = float.Parse (info [3]);
		target.y = float.Parse (info [4]);

		if (m_alive == 1) {
			Summon ();
		}
	}
}
