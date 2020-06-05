using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void GotoRegister()
    {
        SceneManager.LoadScene(1);
    }
    public void GotoLogin()
    {
        SceneManager.LoadScene(2);
    }
    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }
}
