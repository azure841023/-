using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Behavior : MonoBehaviour {

	//除了自己以外的玩家的控制，從伺服器接收訊息執行
	private Animator animator;
	private Rigidbody2D rb;
	public string playername;
	Anima2D.SpriteMeshAnimation head, body, weapon;

	[Header("各種狀態偵測")]
	[SerializeField]				//顯示private
	private bool walk;				//行走中
	//[HideInInspector]				//隱藏public
	public bool onground = false;	//踩在地面，CheckOnGround程式控制
	//[HideInInspector]
	public bool hitwall = false;	//撞牆，CheckHitWall程式控制
	//[HideInInspector]
	public bool Attack = false;		//攻擊
	public NewPlayer newplayer;
	public int check_online;

	private float sendtime = 0.1f;		//多久傳一次訊息
	float send_curtime ;


	[Header("目標位置")]
	public Vector2 position;		//自己應到的位置

	char[] deleteChars = { ' ', ',', '=', ':', '\t' };	//欲刪除之符號集合

	//bool otherplayerBuffer = true;



	void Start () {
		animator = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		walk = false;
		send_curtime = sendtime;
		newplayer = GameObject.Find ("其他玩家群").GetComponent<NewPlayer> ();
	}

	void Update () {
		move ();
		attack ();

		//StartCoroutine (GetInfo ("na", playername));
		if (send_curtime <= 0 ){
			StartCoroutine(GetInfo("sn",playername));		//執行php中的 sn，以自己名字查詢位置
			send_curtime = sendtime;
		}
	}

	void FixedUpdate(){
		send_curtime -= Time.deltaTime;
	}

	void move(){
		//控制面向方向
		if (position.x != rb.position.x)
			walk = true;
		else
			walk = false;
		animator.SetBool ("walk", walk);	//走路動畫

		//移動 (撞到牆壁仍繼續前進時，只播走路動畫但不移動)
		float direction = position.x - rb.position.x;
		if (direction > 0) {
			rb.transform.rotation = Quaternion.Euler (0, 0, 0);
		} else if (direction < 0) {
			rb.transform.rotation = Quaternion.Euler (0, 180f, 0);
		}

		if (position.y != rb.position.y) {
			onground = false;
		}
		else
			onground = true;
		animator.SetBool ("OnGround", onground);

		rb.transform.position = new Vector3 (position.x, position.y, rb.transform.position.z);
	}
	void attack(){
		animator.SetBool ("Attack", Attack);
        Attack = false;
	}
    public void ApplyAttack(bool att) {
        Attack = att;
    }

	void Applyname (string name){
		playername = name;
	}
	void Applybornposition (Vector2 pos){
		gameObject.transform.position = new Vector3 (pos.x, pos.y, gameObject.transform.position.z);
		position = new Vector3 (pos.x, pos.y, gameObject.transform.position.z);
	}
	public void Applyhead(int num){
        head = gameObject.transform.Find("spritemesh/頭").GetComponent<Anima2D.SpriteMeshAnimation>();
        head.frame = num;
    }
	public void Applybody(int num){
        body = gameObject.transform.Find("spritemesh/身體").GetComponent<Anima2D.SpriteMeshAnimation>();
        body.frame = num;
    }
	public void Applyweapon(int num){
        weapon = gameObject.transform.Find("spritemesh/weapon").GetComponent<Anima2D.SpriteMeshAnimation>();
        weapon.frame = num;
    }


	IEnumerator GetInfo(string opcode, string name){
		bool otherplayerBuffer = true;

		WWWForm form = new WWWForm ();
		form.AddField ("number", PlayerAccount.usernum);
		form.AddField ("differ", opcode);
		form.AddField ("name", name);

        if (otherplayerBuffer == true)
        {
            otherplayerBuffer = false;
            WWW www = new WWW("http://140.136.150.77/updata1.php", form);
            yield return www;
//			Debug.Log (www.text);

            if (www.text != null && www.text != "")
            {
                string[] info = www.text.Split(deleteChars);
                //Debug.Log ("Be:"+info [0] + "  " + info [1] + "  " + info [2]);
				position.x = float.Parse(info[1]);
				position.y = float.Parse (info [2]);
				check_online = int.Parse (info [9]);
                otherplayerBuffer = true;
            }
        }
		if (check_online == 0) {
            int num = newplayer.viewedPlayer.IndexOf(playername);
			newplayer.viewedPlayer.Remove (playername);
            newplayer.someplayer.RemoveAt(num);
			Destroy (gameObject);
		}
	}
}