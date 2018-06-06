using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.UI;

public class playerbehave : MonoBehaviour {

	//網路聯繫用
	#region 網路
	const string ip = "140.136.150.77";
	const int _port = 8877;
//	static int attempts = 0;
	private static Socket _clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	private bool isSend;
	private bool isReceive;
	public static IPAddress ipAddress = IPAddress.Parse(ip);
	public static IPEndPoint ipPoint = new IPEndPoint (ipAddress, _port);
	int DataLong = 100;
	string s;
//	string account = "Test";
	#endregion

	//聊天室相關
	#region 聊天室
	string Textbuffer;									//接收到的外部字串
	bool ReceiveControl = false;
	char[] deleteChars = { ' ', ',', '=', ':', '\t' };	//欲刪除之符號集合
	GameObject GetInputString;							//間接接取輸入字
	keyboard keyboard;									//上者的keyboard程式
	#endregion


	private Animator animator;
	private Rigidbody2D rb;

	[Header("各種狀態偵測")]
	[SerializeField]				//顯示private
	private bool walk;				//行走中
	//[HideInInspector]				//隱藏public
	public bool onground = false;	//踩在地面，CheckOnGround程式控制
	//[HideInInspector]
	public bool hitwall = false;	//撞牆，CheckHitWall程式控制
	//[HideInInspector]
	public bool Attack = false;		//攻擊
	//[HideInInspector]
	public bool CanTranslate = false;		//可否傳送
	public bool CanMove = true;
	public bool death = false;		//死亡

	[Header("走路速度")]
	public float speed = 10f;
	private float ws;
	[Header("跳躍高度")]
	public float high = 10f;

	[Header("角色資料")]
	public string name;
	public int level;
	Anima2D.SpriteMeshAnimation head, body, weapon;
	//[SerializeField]
	[HideInInspector]
	public float maxHP = 100f;
	public float HP = 100f;
	public float reHP = 3f;			//每次回血量
	[SerializeField]
	private float ReTime = 10f;		//每次回血時間間隔
	//[SerializeField]
	[HideInInspector]
	public float maxExp = 100f;
	public float Exp = 0f;			//經驗值
	float re_curtime;
	public ChangeWeapon weapon_spritemesh;
	public GameObject picture;
	public GameObject rip;
	public GameObject die_panel;
	[Header("CD時間")]
	public float CDTime = 3f;
	float cd_curtime;
	[Header("無敵")]
	[SerializeField]
	private bool invincible = false;
	private float invintime = 2f;	//無敵時間
	float in_curtime;
	public GameObject hit;
	Anima2D.SpriteMeshAnimation hit_head, hit_body, hit_weapon;
	bool flashing = false;			//控制被攻擊的閃爍效果
	[Header("下個傳送位置")]
	[SerializeField]
	private Vector2 nextposition;
	[Header("UI")]
	#region UI條、玩家頭像、資料
	public Slider hpBar;		//HP條
	public Text hpText;			//HP顯示字
	public Slider mpBar;		//MP條
	public Text mpText;			//MP顯示字
	public Slider expBar;		//EXP條
	public Text expText;		//EXP顯示字
	public Text share;			//聊天室視窗字
	public GameObject info_text;//左上角色資料
	public GameObject info_head;//左上角色頭像
    #endregion
    [Header("其他玩家群")]
    public GameObject otherplayer;


    static bool updateBuffer = true;


	[Header("網路連線用")]
	private float sendtime = 0.1f;		//多久傳一次訊息
	float send_curtime;



	void Start () {
		head = gameObject.transform.Find("spritemesh/頭").GetComponent<Anima2D.SpriteMeshAnimation>();
		body = gameObject.transform.Find("spritemesh/身體").GetComponent<Anima2D.SpriteMeshAnimation>();
		weapon = gameObject.transform.Find("spritemesh/weapon").GetComponent<Anima2D.SpriteMeshAnimation>();
		hit_head = hit.transform.GetChild(1).GetComponent<Anima2D.SpriteMeshAnimation> ();
		hit_body = hit.transform.GetChild(2).GetComponent<Anima2D.SpriteMeshAnimation> ();
		hit_weapon = hit.transform.GetChild(6).GetComponent<Anima2D.SpriteMeshAnimation> ();
		StartCoroutine (GetInfo ("sn", PlayerAccount.ACCOUNT));		//讀取腳色上次登出時的位置作為起始位置

		animator = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		walk = false;
		hpBar.value = maxHP / maxHP;
		GetInputString = GameObject.Find("間接接取輸入字");
		keyboard = GetInputString.GetComponent<keyboard> ();

		rip.SetActive (false);
		die_panel.SetActive (false);



		//網路連線
		ConnectServer ();
		send_curtime = sendtime;
		Send("1,"+PlayerAccount.ACCOUNT);
		//Push ("n", gameObject.transform.position.x.ToString(), gameObject.transform.position.y.ToString(), "patrick");
	}
	

	void Update () {
		hpBar.value = HP / maxHP;
		hpText.text = "HP: " + HP + " / " + maxHP;
		expBar.value = Exp / maxExp;
		expText.text = "EXP: " + Exp + " / " + maxExp;

		if (death != true) {
			move ();
			jump ();
			attack ();
			recovery ();
		


			if (in_curtime <= 0) 				//無敵時間結束
			invincible = false;
			if (invincible == true) {
				hit.SetActive (flashing);
				flashing = !flashing;
			} else
				hit.SetActive (false);

			if (send_curtime <= 0) {
				if (updateBuffer == true)
					Updata ("u", PlayerAccount.ACCOUNT, gameObject.transform.position.x.ToString (), gameObject.transform.position.y.ToString ());
				//執行php中的 u (update)，更新自己在資料庫中的座標
				send_curtime = sendtime;
			}
		}

		if (ReceiveControl == true) {		//若接收到訊息(非自己傳的)則更新聊天室內容
			ReceiveText (Textbuffer);		//將暫存字串更新到聊天室
		}
		if (Exp >= maxExp) {
			Exp = 0f;
			level++;
			info_text.SendMessage("Applylv", level);
			StartCoroutine (levelup (level));
		}
	}

	//處理冷卻時間
	void FixedUpdate(){
		cd_curtime -= Time.deltaTime;		//CD時間
		re_curtime -= Time.deltaTime;		
		in_curtime -= Time.deltaTime;
		send_curtime -= Time.deltaTime;
	}

	void move(){	//傳id,XY座標
		//控制面向方向
		if (CanMove == true) {
			if (Input.GetKey ("right") || Input.GetKey ("left"))
				walk = true;
			else
				walk = false;

			animator.SetBool ("walk", walk);	//走路動畫

			//移動 (撞到牆壁仍繼續前進時，只播走路動畫但不移動)
			ws = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
			//Debug.Log("input.getaxis:"+Input.GetAxis ("Horizontal")+"\nspeed:"+speed+"\nTime.deltaTime:"+ Time.deltaTime+"\n");
			if (ws > 0) {
				rb.transform.rotation = Quaternion.Euler (0, 0, 0);
				if (!hitwall)
					rb.transform.Translate (ws, 0, 0);
			} else if (ws < 0) {
				rb.transform.rotation = Quaternion.Euler (0, 180f, 0);
				if (!hitwall)
					rb.transform.Translate (-ws, 0, 0);
			}
		}
	}

	//跳躍+傳送 (按上的功能)
	void jump(){
		if (CanTranslate == false) {
			if (Input.GetKey ("up") && onground == true) {
				rb.velocity = new Vector2 (rb.velocity.x, high);
				onground = false;
			}
			animator.SetBool ("OnGround", onground);
		} else {
			if (Input.GetKeyUp("up")) {
				rb.transform.position = new Vector3 (nextposition.x, nextposition.y);
				CanTranslate = false;
			}
		}
	}
	//攻擊
	void attack(){	//傳id,攻擊布林值
		if (Input.GetKeyDown("space") && Attack == false) {
			Attack = true;
            Send("H," + name );
		}
		animator.SetBool ("Attack", Attack);
		Attack = false;
	}

	//自動回血
	void recovery(){
		if (re_curtime <= 0) {
			if (HP < maxHP)
				HP += reHP;
			re_curtime = ReTime;
		}
		if (HP > maxHP)
			HP = maxHP;
	}

	//被攻擊
	void ApplyDamage(int harm){
		if (invincible == false) {
			CanMove = false;
			rb.velocity = Vector2.zero;
			if (rb.transform.rotation.y == 0)
				rb.velocity = new Vector2 (-speed / 1.5f ,high / 2f);
			else 
				rb.velocity = new Vector2 (speed / 1.5f, high / 2f);

			HP -= harm;
			if (HP <= 0) {
				HP = 0;
				die ();
			}
			invincible = true;
			in_curtime = invintime;
			StartCoroutine (wait (1f));
		}
	}
	//死亡
	void die(){
		death = true;
		picture.SetActive (false);
		rip.SetActive (true);
		die_panel.SetActive (true);
	}

	//傳送
	#region Transport
	void ApplyTranslate_bool(bool trans){
		CanTranslate = trans;
	}
	void ApplyTranslate_pos(Transform pos){
		nextposition = new Vector3 (pos.transform.position.x, pos.transform.position.y, gameObject.transform.position.z);
	}
	#endregion












	//網路連線
	/*private static void SendServer (string message){
		if (message == null)
			Debug.Log ("message = Null");
		else if (LoopConnect () == true)
			_clientSocket.Send (Encoding.ASCII.GetBytes (message));
	}
	private static bool LoopConnect(){
		if (!_clientSocket.Connected) {
			try{
				_clientSocket.Connect(ipPoint);
				attempts++;
			}
			catch (SocketException){
	//			Debug.Log ("Connect attempts: " + attempts.ToString ());
			}
		}
		if (_clientSocket.Connected) {
	//		Debug.Log ("Connected");
			return true;
		} else {
	//		Debug.Log ("Not Connected");
			return false;
		}
	}*/
	private void ConnectServer(){
		try{
//		Debug.Log("Connected...1");
			_clientSocket.Connect(ipPoint);
//		Debug.Log("Connecter...1.1");
			Thread ThreadCl = new Thread(SockClientReveive);
			ThreadCl.Start();
		}
		catch{}
	}

	private void SockClientReveive(){
		try{
//		Debug.Log("Connected...2");
			long IntAcceptData;							//接收到的socket的單詞個數
			byte[] clientData = new byte[DataLong];		//上限 100 (DataLong)的單詞數容量
//		Debug.Log("Connected...3");
			while(true){
//				Debug.Log("Connected...3.1");
				IntAcceptData = _clientSocket.Receive(clientData);
//				Debug.Log("Connected...3.2");
//				Debug.Log(IntAcceptData);
//				Debug.Log("Connected...4.2");
				s = Encoding.Default.GetString(clientData);		//所接收到的所有字串單詞
				s = s.Substring(0,(int)IntAcceptData);			//依長度修改為本身的子字串
				Debug.Log("s = " + s);
				Textbuffer = s;									//暫存字串
				ReceiveControl = true;
//				Debug.Log("Connected...4.3");

			}
		}
		catch{}
	}
	private void ReceiveText(string s){									//所接收到的字串
		string[] message = s.Split (',');                               //以,切割字串
        if (message[0] == "2")
        {
            PlayerAccount.usernum = int.Parse(message[1]);
            //Debug.Log (PlayerAccount.usernum);
        }
        else if (message[0] == "H") {
			//Debug.Log ("H," + message [1]);
			if (message[1] != name)
            	otherplayer.SendMessage("ApplyHit", message[1]);
        }
        else if (message[0] != PlayerAccount.ACCOUNT && message[0] != "2" )
        {                       //若訊息發件人非自己
            share.text += message[0] + ":" + message[1] + "\n";
            keyboard.line.Add(message[0] + ":" + message[1] + "\n");    //修改keyboard中的聊天室訊息紀錄
                                                                        //Debug.Log ("line.Count=" + keyboard.line.Count);
            if (keyboard.line.Count > keyboard.linenumber)
            {
                share.text = "";
                for (int i = 0; i < keyboard.linenumber; i++)
                {
                    keyboard.line[i] = keyboard.line[i + 1];
                    share.text += keyboard.line[i];
                }
                keyboard.line.RemoveAt(keyboard.linenumber);
            }
        }
		ReceiveControl = false;
	}
	public void Send (string InputString){
		try{
			string SendS = InputString;
			_clientSocket.Send(Encoding.ASCII.GetBytes(SendS));
		}
		catch{}
	}
	private void OnApplicationQuit(){
		_clientSocket.Close ();
	}
	private static void Push(string sql, string x, string y,string account)
	{
	//	Debug.Log("Starting....Push...1");
		WWWForm form = new WWWForm();
	//	Debug.Log("Starting....Push...1.1");
		form.AddField("differ", sql);
	//	Debug.Log("Starting....Push...2");
		form.AddField("account", account);
	//	Debug.Log("Starting....Push...3");
	//	form.AddField("password", "10/17");
	//	Debug.Log("Starting....Push...3.1");
	//	Debug.Log("Starting....Push...4");
		form.AddField("x", x);
		form.AddField("y", y);
	//	Debug.Log("Starting....Push...5");
	//	form.AddField("accound", "accoundtest");
	//	Debug.Log("Starting....Push...6");
		WWW www = new WWW("http://140.136.150.77/updata1.php", form);
		if (!string.IsNullOrEmpty(www.error))
		{
	//		Debug.Log(www.error);
		}
		else
		{
	//		Debug.Log("Finished Uploading Screenshot");
		}
	}
	private static void Updata(string sql, string uaccount, string x, string y) 
	{ 
	//	Debug.Log("Starting....Push...1"); 
		WWWForm form = new WWWForm(); 
	//	Debug.Log("Starting....Push...1.1"); 
		form.AddField("number",PlayerAccount.usernum);
		form.AddField("differ", sql); 
	//	Debug.Log("Starting....Push...2"); 
		form.AddField("uaccount", uaccount); 
	//	Debug.Log("Starting....Push...3"); 
	// 	form.AddField("password", "10/17"); 
	//	Debug.Log("Starting....Push...3.1"); 
	//	form.AddField("uaccount", uaccount); 
	//	Debug.Log("Starting....Push...7"); 
		form.AddField("x", x); 
	//	Debug.Log("Starting....Push...8"); 
		form.AddField("y", y); 
	//	Debug.Log("Starting....Push...9"); 
		WWW www = new WWW("http://140.136.150.77/updata1.php", form); 
		//Debug.Log (www.text);
		if (!string.IsNullOrEmpty(www.error)) 
		{ 
		//	Debug.Log(www.error); 
		} 
		updateBuffer = true;
	}
	private static void Delete(string sql, string delete)
	{
	//	Debug.Log("Starting....Push...1");
		WWWForm form = new WWWForm();
	//	Debug.Log("Starting....Push...1.1");
		form.AddField("differ", sql);
	//	Debug.Log("Starting....Push...2");
	//	Debug.Log("Starting....Push...3");
		form.AddField("password", "10/17");
	//	Debug.Log("Starting....Push...3.1");
		form.AddField("delete", delete);
	//	Debug.Log("Starting....Push...4");
		WWW www = new WWW("http://140.136.150.77/updata1.php", form);
		if (!string.IsNullOrEmpty(www.error))
		{
	//		Debug.Log(www.error);
		}
		else
		{
	//		Debug.Log("Finished Uploading Screenshot");
		}
	}

	IEnumerator GetInfo(string opcode, string username){

		WWWForm form = new WWWForm ();
		form.AddField ("differ", opcode);
		form.AddField ("account", username);
		WWW www = new WWW ("http://140.136.150.77/start2.php", form);
		yield return www;

        //Debug.Log (www.text);


        //www字串切割
        if (www.text != null)
        {
            string[] info = www.text.Split(deleteChars);
            name = info[0];
			info_text.SendMessage ("Applyname", name);
            gameObject.transform.position = new Vector3(float.Parse(info[1]), float.Parse(info[2]), 0f);
			head.frame = int.Parse(info[3]);
			info_head.SendMessage ("Applyhead", head.frame);
            body.frame = int.Parse (info [4]);
            weapon.frame = int.Parse (info [5]);
            level = int.Parse(info[6]);
            info_text.SendMessage("Applylv", level);
			HP = float.Parse (info [7]);
			Exp = float.Parse (info [8]);

			maxHP = 100f + (level - 1) * 10f;
			maxExp = 100f + (level - 1) * 20f;
			weapon_spritemesh.SendMessage ("ApplyDamage", level);
			hit_head.frame = head.frame;
			hit_body.frame = body.frame;
			hit_weapon.frame = weapon.frame;
        }
	}

	IEnumerator levelup(int lv){
		WWWForm form = new WWWForm ();
		form.AddField ("number", PlayerAccount.usernum);
		form.AddField ("differ", "lv");
		form.AddField ("account", PlayerAccount.ACCOUNT);
		form.AddField ("level", lv);
		WWW www = new WWW ("http://140.136.150.77/updata1.php", form);
		yield return www;

		maxHP = 100f + (level - 1) * 10f;
		maxExp = 100f + (level - 1) * 20f;
		weapon_spritemesh.SendMessage ("ApplyDamage", level);
	}

	IEnumerator wait(float t){
		#region 等待t秒恢復可移動
		yield return new WaitForSeconds (t);
		if (onground == true) {
			rb.velocity = Vector2.zero;
			CanMove = true;
		}
		CanMove = true;
		#endregion
	}
}
