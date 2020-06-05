using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;

    public Button submitButton;

    public TMP_Text wwwtext;

    public void CallBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CallLogin()
    {
        StartCoroutine(Login());
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);

        WWW www = new WWW("192.168.0.20/sqlconnect/login.php", form);
        yield return www;
        if (www.text == "0")
        {
            //If logged in fine log in and set the username to make it easier to access
            DBManager.username = username.text;
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        } else
        {
            Debug.LogError("log in failed error:" + www.text);
        }
        
    }
    public void VerifyInputs()
    {
        //TODO make this system better
        submitButton.interactable = (username.text.Length <= 10 &&
                                     password.text != "");
    }


}
