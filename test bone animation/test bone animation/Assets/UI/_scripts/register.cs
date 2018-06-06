using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class register : MonoBehaviour {
    public string inputusername;
    public string inputpassword;
    public InputField account;
    public InputField password;
    public bool buffer = false;
	public GameObject error, unconnect;
    // Use this for initialization
    void Start () {
		error.SetActive (false);
		unconnect.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
       
    }
	public void StartCoroutine(string SceneName)
    {
		StartCoroutine(Login(account.text, password.text, SceneName));
		//StartCoroutine(RegisterAccount(account.text, password.text, SceneName));
		//StartCoroutine(trylog());
    }

	/*IEnumerator trylog(string nextScene){
		WWWForm form = new WWWForm();
		form.AddField("user", account.text);
		WWW www = new WWW("http://140.136.150.77/account.php", form);
		yield return www;
	}*/

	IEnumerator Login(string username, string password, string nextScene)
    {
        WWWForm form = new WWWForm();
		form.AddField ("differ", "ss");
		form.AddField ("account", username);
		WWW www = new WWW ("http://140.136.150.77/start1.php", form);
        yield return www;
		//Debug.Log("www : "+www.text);

		if (www.text.Equals ("1")) {
			buffer = false;
			unconnect.SetActive (false);
			error.SetActive (true);
		} else if (www.text.Equals ("0")) {
			buffer = true;
			error.SetActive (false);
			//傳送username跟password給資料庫，新增帳戶
			WWWForm table = new WWWForm ();
			table.AddField ("differ", "n");
			table.AddField ("account", username);
			PlayerAccount.ACCOUNT = username;
			table.AddField ("password", password);
			WWW wwwt = new WWW ("http://140.136.150.77/start1.php", table);
			yield return wwwt;
			//Debug.Log (password);
			//Debug.Log (wwwt.text);

			SceneManager.LoadScene (nextScene);
		}
        //Debug.Log(buffer);
		else
			unconnect.SetActive (true);
    }
	/*IEnumerator RegisterAccount(string username, string password, string nextScene)
    {
        if (buffer){
			//傳送username跟password給資料庫，新增帳戶
            WWWForm form = new WWWForm();
            form.AddField("differ", "n");
            form.AddField("account", username);
			PlayerAccount.ACCOUNT = username;
            form.AddField("password", password);
			WWW www = new WWW("http://140.136.150.77/logic2.php", form);


            yield return www;
			//Debug.Log (www.text);
			//Debug.Log ("Success");


			//Application.LoadLevel(nextScene);
			SceneManager.LoadScene(nextScene);//讀取場景,場景名稱
        }
    }*/
}
