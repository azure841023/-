using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerView : MonoBehaviour {

	private Animator animator;
	private Rigidbody2D rb;

	[Header("各種狀態偵測")]
	[SerializeField]				//顯示private
	private bool walk;				//行走中
	//[HideInInspector]
	public bool Attack = false;		//攻擊
	//[HideInInspector]

	[Header("走路速度")]
	public float speed = 10f;
	private float ws;
	[Header("跳躍高度")]
	public float high = 10f;

	[Header("角色資料")]
	public string name;


	[Header("CD時間")]
	public float CDTime = 3f;
	float cd_curtime;
	[Header("無敵")]
	[SerializeField]
	private bool invincible = false;
	private float invintime = 2f;	//無敵時間
	float in_curtime;
	public GameObject hit;
	bool flashing = false;			//控制被攻擊的閃爍效果


	void Start () {
		animator = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		walk = false;
		animator.SetBool ("OnGround", true);
	}


	void Update () {
		move ();
		attack ();
		if (in_curtime <= 0) 				//無敵時間結束
			invincible = false;
		if (invincible == true) {
			hit.SetActive (flashing);
			flashing = !flashing;
		} else
			hit.SetActive (false);
	}
	void FixedUpdate(){
		cd_curtime -= Time.deltaTime;		//CD時間	
		in_curtime -= Time.deltaTime;
	}

	void move(){	//傳id,XY座標
		//控制面向方向
		if (Input.GetKey ("right") || Input.GetKey ("left"))
			walk = true;
		else
			walk = false;

		animator.SetBool ("walk", walk);	//走路動畫

		//移動 (撞到牆壁仍繼續前進時，只播走路動畫但不移動)
		ws = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
		if (ws > 0) {
			rb.transform.rotation = Quaternion.Euler (0, 0, 0);
			//if (!hitwall)
				//rb.transform.Translate (ws, 0, 0);
		} else if (ws < 0) {
			rb.transform.rotation = Quaternion.Euler (0, 180f, 0);
			//if (!hitwall)
				//rb.transform.Translate (-ws, 0, 0);
		}
	}

	//攻擊
	void attack(){	//傳id,攻擊布林值
		if (Input.GetKey("space")) {
			Attack = true;
		}
		animator.SetBool ("Attack", Attack);
		Attack = false;
	}

	//被攻擊
	void ApplyDamage(int harm){
		if (invincible == false) {
			rb.velocity = Vector2.zero;
			if (rb.transform.rotation.y == 0)
				rb.velocity = new Vector2 (-speed ,high / 1.5f);
			else 
				rb.velocity = new Vector2 (speed, high / 1.5f);

			invincible = true;
			in_curtime = invintime;
		}
	}
	//死亡
	void die(){

	}
}