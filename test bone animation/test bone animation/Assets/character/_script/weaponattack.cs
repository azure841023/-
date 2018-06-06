using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponattack : MonoBehaviour {

	public float damage;		//攻擊力，由ChangeWeapon程式控制
	//private bool canhit;		//確保揮一下只造成一次傷害
	public playerbehave player;

	void Start () {
	//	canhit = true;
	}

	void Update () {
		
	}
	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Monster" /*&& canhit == true*/) {
			other.SendMessage ("ApplyDamage", damage);
			other.SendMessage ("ApplyWhoHit", player);
			//canhit = false;
		}
	}
		
	/*void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Monster") {
			canhit = true;
		}
	}*/
}
