using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterbehavior : MonoBehaviour {

	public enum type{		//怪物的種類(選項式)
		dog,slime,spider,chicken
	}
	public type monstertype;

	public int monsterID;
	public bool dead;
	public float maxhp;
	[SerializeField]
	private float hp;
	public bool destroy;	//animator控制
	public bool walk;		//走路
	[SerializeField]
	private bool hit;		//是否被攻擊(false由animator控制)
	[SerializeField]
	private float damage;	//怪物攻擊力
	public playerbehave whoHit;
	[SerializeField]
	private float exp;
	Animator animator;
	public GenerateMonster NewMonster;	//死後呼叫產生下一隻

	[Header("目標位置")]
	public Vector2 position;

	char[] deleteChars = { ' ', ',', '=', ':', '\t' };	//欲刪除之符號集合

	GameObject character;								//玩家物件
	playerbehave script;								//玩家物件中的playbehave程式(為了使用send功能)

	private float sendtime = 1f;		//多久傳一次訊息
	float send_curtime;
    //static bool generateBuffer = true;


	void Start () {
		hp = maxhp;
		dead = false;
		destroy = false;
		hit = false;
		animator = gameObject.GetComponent<Animator>();
		character = GameObject.Find ("character");
		script = character.GetComponent<playerbehave> ();
		send_curtime = sendtime;

		if (monstertype == type.dog) {					//依種類設定怪物攻擊力
			damage = 10;
			maxhp = 100f;
			exp = 20f;
		} else if (monstertype == type.chicken) {
			damage = 15;
			maxhp = 120f;
			exp = 50f;
		} else if (monstertype == type.spider) {
			damage = 20;
			maxhp = 150f;
			exp = 80f;
		} else if (monstertype == type.slime) {
			damage = 30;
			maxhp = 200f;
			exp = 200f;
		}
	}

	void Update () {
		move ();

		if (destroy == true)
			Destroy (gameObject);
		animator.SetBool ("hit", hit);					//被打動畫

		if (send_curtime <= 0 ){
			StartCoroutine (GetInfo ("sxy", monsterID));	//執行php中的 sxy，依照所傳送ID查詢自己資料庫中的座標
			send_curtime = sendtime;
		}

		if (gameObject.transform.position.y == 0) {			//鎖死怪物y座標
			if (monstertype == type.dog) {
				gameObject.transform.position = new Vector3 (gameObject.transform.position.x, -11f, gameObject.transform.position.z);
			} else if (monstertype == type.chicken) {
				gameObject.transform.position = new Vector3 (gameObject.transform.position.x, -715f, gameObject.transform.position.z);
			} else if (monstertype == type.spider) {
				gameObject.transform.position = new Vector3 (gameObject.transform.position.x, -459f, gameObject.transform.position.z);
			} else if (monstertype == type.slime) {
				gameObject.transform.position = new Vector3 (gameObject.transform.position.x, -230f, gameObject.transform.position.z);
			}
		}
	}

	void FixedUpdate(){
		send_curtime -= Time.deltaTime;
	}

	void move(){
		if (position.x != gameObject.transform.position.x)
			walk = true;
		else
			walk = false;
		animator.SetBool ("walk", walk);				//走路動畫

		float direction = position.x - gameObject.transform.position.x;
//        Debug.Log(direction);
		if (Mathf.Abs (direction) <= 1) {
			gameObject.transform.position = new Vector3 (position.x, position.y, gameObject.transform.position.z);
		}
        else if (direction > 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 180f, 0);
            gameObject.transform.Translate(-10f * Time.deltaTime, 0f, 0f);
        }
        else if (direction < 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.Translate(-10f * Time.deltaTime, 0f, 0f);
        }

		//gameObject.transform.position = new Vector3 (position.x, position.y, gameObject.transform.position.z);

	}

	void ApplyDamage(int harm){							//接收從玩家武器傳來的攻擊力數值
		hp -= harm;
		hit = true;
		if (hp <= 0)
			die ();
		script.Send ("A," + monsterID);
		StartCoroutine (updateMonsterHp ("uh", monsterID, hp));
	}

	void ApplyID(int id){
		monsterID = id;
	}

	void ApplyWhoHit(playerbehave player){
		whoHit = player;
	}

	void die(){
		if (dead == false) {
			whoHit.Exp += exp;
			dead = true;
			script.Send ("M," + monsterID);
			NewMonster.SendMessage ("ApplyNew", monsterID);
		}
		animator.SetBool ("die", dead);
	}

	void ApplyBorn(GenerateMonster script){
		NewMonster = script;
	}

	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "Player" ) {
			other.SendMessage ("ApplyDamage", damage);
		}
	}

	IEnumerator GetInfo(string opcode, int id){
		WWWForm form = new WWWForm ();
		form.AddField ("differ", opcode);
		form.AddField ("id", id);

        //if (generateBuffer == true)
        //{
        //    generateBuffer = false;
            WWW www = new WWW("http://140.136.150.77/monster.php", form);
            yield return www;

            //Debug.Log (www.text);

        //www字串切割

        if (www.text != null && www.text != "")
        {
            //Debug.Log (info.Length);
            string[] info = www.text.Split(deleteChars);
            if (!info[0].Equals("<br"))
            {
                //Debug.Log("info[0] : = " + info[0]);
                //Debug.Log("info[1] : = " + info[1]);
				//if (transform.position.y == 0)
				//	transform.position = new Vector3 (float.Parse (info [1]), float.Parse (info [2]), transform.position.z);
				position = new Vector2 (float.Parse (info [1]), float.Parse (info [2]));
				
                //Debug.Log("info[2] : = " + info[2]);
				hp = float.Parse(info[3]);
            }
			if (hp <= 0) {
				NewMonster.SendMessage ("ApplyNew", monsterID);
				Destroy (gameObject);
			}
        }

        //    generateBuffer = true;
     //   }
	}
	IEnumerator updateMonsterHp(string opcode, int id, float hp){
		WWWForm form = new WWWForm ();
		form.AddField ("differ", opcode);
		form.AddField ("hp", (int)hp);
		form.AddField ("id", id);
		WWW www = new WWW ("http://140.136.150.77/monster.php", form);
		yield return www;
	}
    IEnumerator wait() {
        yield return new WaitForSeconds(1);
    }
}
