using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class profilesearch : MonoBehaviour
{
    public Displaycontroller dc;
    public TMP_Text usersearchtext;
    public Button searchButton;
    public GameObject profilePanel;
    public GameObject buttonPrefab;
    public GameObject[] buttons;
    private void Start()
    {
        //Close messages and show profile
        dc.messageBox.SetActive(false);
        dc.msgboxPanel.SetActive(false);
        ProfileShow();
        buttons = null;
        dc.cards = null;

    }

    public void ProfileShow()
    {
        //TODO AUTOMATE THIS I HATE SEEING THIS OMFG
        dc.bodyPanels[0].gameObject.SetActive(true);
        dc.bodyPanels[1].gameObject.SetActive(true);
        profilePanel.gameObject.SetActive(false);
        dc.shopBody.gameObject.SetActive(false);
        dc.cardMenu.gameObject.SetActive(false);
        dc.msgboxPanel.SetActive(false);
        dc.selectCardFight.SetActive(false);
        dc.CloseDojo();
        //If not logged in logout
        if (DBManager.username == null)
        {
            SceneManager.LoadScene(0);
            DBManager.Logout();
            
        }
        else
        {
            //Load user data 
            StartCoroutine(dc.LoadUserData(DBManager.username));
            //Set sidebar text
            //TODO add currency to sidebar
            dc.sidebarText.text = DBManager.username;
            
        }


    }

    

    public void CallSearch()
    {
        StartCoroutine(SearchUsers());
    }


    IEnumerator SearchUsers()
    {
        //Get users from the text input and search them on the DB
        WWWForm form = new WWWForm();
        form.AddField("searchname", usersearchtext.text);
        Debug.Log("Search database for username of: " + usersearchtext.text);
        WWW www = new WWW("192.168.0.20/sqlconnect/search.php", form);
        yield return www;
        string[] webResults = www.text.Split('\t');
        ProfileSearch(webResults);
    }


    void ProfileSearch(string[] results)
    {
        //TODO AUTOMATE THIS I CANT RE WRITE THIS AGAIN
        dc.bodyPanels[0].gameObject.SetActive(false);
        dc.bodyPanels[1].gameObject.SetActive(false);
        dc.DojoPanel.SetActive(false);
        dc.shopBody.gameObject.SetActive(false);
        profilePanel.SetActive(true);
        dc.cardMenu.gameObject.SetActive(false);
        dc.msgboxPanel.SetActive(false);
        dc.selectCardFight.SetActive(false);

        //kill all children
        int childs = profilePanel.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(profilePanel.transform.GetChild(i).gameObject);
            buttons = null;
        }


        foreach (string s in results)
        {
            if (s != "")
            {
                //Load card to page
                GameObject btn = Instantiate(buttonPrefab, profilePanel.transform);
                TMP_Text btntxt = btn.GetComponentInChildren<TMP_Text>();
                //TODO show only select data and neaten it up
                btntxt.text = s;

            }
        }


        if (buttons == null)
        {
            //if profile button dont exist find them
            buttons = GameObject.FindGameObjectsWithTag("pButton");
            
        }
        int bIndex = 0;
        foreach (GameObject button in buttons)
        {
            //each button show the user and when clicked goto other profile
            Button b = button.GetComponent<Button>();
            string bText = button.GetComponentInChildren<TMP_Text>().text;
            bText = results[bIndex];
            b.onClick.AddListener(delegate { test(b); });
            bIndex++;
            
        }
    }

    //TODO Rename this from test like cmon
    void test(Button b)
    {
        
        
        //Load profile of clicked user
        string btext = b.GetComponentInChildren<TMP_Text>().text;
        Debug.Log(b);
        LoadProfilePanel(btext);
        //Kill chuldren
        int childs = profilePanel.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(profilePanel.transform.GetChild(i).gameObject);
        }
    }

    void LoadProfilePanel(string b)
    {
        //Close all other things and load user data
        buttons = null;
        dc.DojoPanel.SetActive(false);
        dc.shopBody.gameObject.SetActive(false);
        profilePanel.SetActive(false);
        dc.StartCoroutine(dc.LoadUserData(b));

    }
}
