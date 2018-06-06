using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour {

	int frame = 0;	//anima2d.SpriteMeshAnimation的Frame值，用來表示第幾號武器
	int old_frame;
	CapsuleCollider2D weapon_blade;
	weaponattack weaponattack;
	int level, new_level;

	int sword = 0;	//劍
	int ax = 1;		//斧頭
	int pick = 2;	//十字鎬


	void Awake () {
		weapon_blade = gameObject.transform.parent.parent.Find ("Bones/hip/torso/R hand/weapon").GetComponent<CapsuleCollider2D>();
        weaponattack = gameObject.transform.parent.parent.Find("Bones/hip/torso/R hand/weapon").GetComponent<weaponattack>();
		ChangeCollider (frame);
	}
	

	void Update () {
		frame = GetComponent<Anima2D.SpriteMeshAnimation> ().frame;
		if (frame != old_frame || level != new_level){		//如果換了武器值
			ChangeCollider (frame);
			level = new_level;
		}
		old_frame = frame;
	}
	void ChangeCollider (int f){
		if (f % 3 == sword) {
			weapon_blade.offset = new Vector2 (3f, -0.45f);
			weapon_blade.size = new Vector2 (4.5f, 1f);
			weaponattack.damage = (f / 3) * 10f + 10f + level * 2f;
		} else if (f % 3 == ax) {
			weapon_blade.offset = new Vector2 (2.8f, -1.35f);
			weapon_blade.size = new Vector2 (2.8f, 1.1f);
			weaponattack.damage = (f / 3) * 4f + 2f + level * 2f;
		} else if (f % 3 == pick) {
			weapon_blade.offset = new Vector2 (3.4f, -1.2f);
			weapon_blade.size = new Vector2 (1f, 2f);
			weaponattack.damage = (f / 3) * 5f + 2f + level * 2f;
		}
	}
	public void ApplyDamage(int lv){
		new_level = lv;
	}
}
