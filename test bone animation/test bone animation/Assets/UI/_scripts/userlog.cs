using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class userlog : MonoBehaviour {
    //public string sence;
    public string inputusername;
    public string inputpassword;
    public InputField account;
    public InputField password;
    public bool buffer = false;
	public GameObject error;

	void Start(){
		error.SetActive (false);
	}

	
	IEnumerator Login(string username,string password,string SceneName)
    {
        WWWForm form = new WWWForm();
        form.AddField("differ", "s");
        form.AddField("account", username);
        form.AddField("password", password);
		WWW www = new WWW("http://140.136.150.77/start1.php", form);
        yield return www;
       // Debug.Log(www.text);
        
        if (www.text.Equals("1")){
            buffer = true;

			WWWForm table = new WWWForm();
			table.AddField("differ", "login");
			table.AddField("account", username);
			WWW wwwt = new WWW("http://140.136.150.77/start2.php", table);
			yield return wwwt;

			SceneManager.LoadScene(SceneName);
        }
        else{
            buffer = false;
			error.SetActive (true);
        }
        //Debug.Log(buffer);
        
    }
	public void Sence(string SceneName)
    {
		PlayerAccount.ACCOUNT = account.text;
		StartCoroutine (Login (account.text, password.text, SceneName));
        //if (buffer){
			//Application.LoadLevel(SceneName);
			//SceneManager.LoadScene(SceneName);
        //}
    }
}
