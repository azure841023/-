using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Package : MonoBehaviour {

	public GameObject bag;

	public void OnClick(){
		bag.SetActive (!bag.activeSelf);
	}

	/*private Image package,item,item1,item3,item2,item4,item5,item6,item7,item8,item9,item10,item11;
	public Text txt;
	public GameObject G;
	private bool isStartTimer;//是否開始計算時間
	// Use this for initialization
	void Start () { 
		package= transform.Find("Package").GetComponent<Image>();//取得FilledSkill內的Image組件	

	}
	// Update is called once per frame
	void Update () {
		if (isStartTimer&&package.fillAmount==1)//如果記時器開始執行
		{
			package.fillAmount = 0;
			G.SetActive (false);
			txt.text =""; 
		}
		else if(isStartTimer&&package.fillAmount==0){
			package.fillAmount = 1;
			G.SetActive (true);
			txt.text = "背包";
		}
		isStartTimer = false;
	} 
	public void OnClick()
	{
		isStartTimer = true;
	}*/
}
