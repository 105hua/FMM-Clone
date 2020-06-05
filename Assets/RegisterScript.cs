using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterScript : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField confirmPassword;
    public TMP_InputField email;
    public TMP_InputField confirmEmail;
    

    public Button submitButton;

    public void CallBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        form.AddField("email", email.text);
        WWW www = new WWW("192.168.0.20/sqlconnect/register.php", form);
        yield return www;

        if (www.text == "0")
        {
            //if successful
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        } else
        {
            //somehow it broke
            Debug.LogError("User creation failed. Error: "+ www.text);
        }
    }

    public void VerifyInputs()
    {
        //TODO Change this to a better system
        submitButton.interactable = (username.text.Length <= 10 &&
                                     confirmPassword.text == password.text &&
                                     password.text != "");
    }
}
