using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;


public class NewPlayer : MonoBehaviour {

	public GameObject OtherPlayer;
	[Header("出現位置")]
	public Vector2 target;
	Vector3 position;
	public string newplayername;
	int head,body,weapon;
	public List<string> viewedPlayer = new List<string> ();			//已顯示玩家清單
    public List<GameObject> someplayer = new List<GameObject>();
    static bool summonBuffer = true;

	public bool trysummon = false;
	char[] deleteChars = { ' ', ',', '=', ':', '\t' };				//欲刪除之符號集合

	private float sendtime = 1f;		//多久傳一次訊息
	float send_curtime ;

	void Start () {
		send_curtime = sendtime;
	}

	void Update () {
	//	Debug.Log("Starting....search...1");
		position = new Vector3(target.x, target.y, 0);


	//	Debug.Log("Starting....search...2");
		if (send_curtime <= 0 ){
			StartCoroutine (GetInfo ("s",PlayerAccount.ACCOUNT));		//執行php中的 s (select)以查詢與自己帳戶不同的其他玩家姓名、位置
			send_curtime = sendtime;
		}
		//	Debug.Log ("trying");
		if (trysummon==true){
			Summon ();
			trysummon = false;
		}
	}

	void FixedUpdate(){
		send_curtime -= Time.deltaTime;
	}

	public void Summon(){											//外部呼叫，用於顯示其他玩家所在
		var newplayer =	Instantiate (OtherPlayer, position, Quaternion.identity);
        newplayer.SendMessage ("Applyname", newplayername);			//將玩家名字傳送給新生角色，並交由其保存
        newplayer.SendMessage ("Applybornposition", new Vector2 (position.x, position.y));	//將玩家顯示座標傳送給新生角色
        newplayer.SendMessage ("Applyhead", head);
        newplayer.SendMessage ("Applybody", body);
        newplayer.SendMessage ("Applyweapon", weapon);
        someplayer.Add(newplayer);
    }


    IEnumerator GetInfo(string opcode, string username) {

        WWWForm form = new WWWForm();
        form.AddField("number", PlayerAccount.usernum);
        form.AddField("differ", opcode);
        form.AddField("account", username);

        if (summonBuffer == true)
        {
            summonBuffer = false;
            WWW www = new WWW("http://140.136.150.77/updata1.php", form);
            yield return www;

//            Debug.Log (www.text);

            //www字串切割

            string[] info = www.text.Split(deleteChars);
            //Debug.Log (info.Length);
            if (!info[0].Equals("<br"))
            {
                for (int i = 0; i < (info.Length - 1) / 6; i++)
                {
                    newplayername = info[6 * i];
                    //Debug.Log(info[6 * i]);
                    target.x = float.Parse(info[6 * i + 1]);
                    //Debug.Log("float.Parse (info [6 * i + 1]) = " + float.Parse(info[6 * i + 1]));
                    target.y = float.Parse(info[6 * i + 2]);
                    //Debug.Log("float.Parse (info [6 * i + 2]) = " + float.Parse(info[6 * i + 2]));
                    head = int.Parse(info[6 * i + 3]);
                    //Debug.Log("int.Parse (info [6 * i + 3]) = " + int.Parse(info[6 * i + 3]));
                    body = int.Parse(info[6 * i + 4]);
                    //Debug.Log("int.Parse (info [6 * i + 4]) = " + int.Parse(info[6 * i + 4]));
                    weapon = int.Parse(info[6 * i + 5]);
                    //Debug.Log("int.Parse (info [6 * i + 5]) = " + int.Parse(info[6 * i + 5]));
                    if (!viewedPlayer.Contains(newplayername))
                    {       //若欲顯示之玩家名字不存在"已顯示玩家清單"裡
                        viewedPlayer.Add(newplayername);
                        Summon();
                    }
                }
            }
        }
        summonBuffer = true; 
     }

    public void ApplyHit(string someone) {
        int num = viewedPlayer.IndexOf(someone);
        someplayer[num].SendMessage("ApplyAttack", true);
    }
}