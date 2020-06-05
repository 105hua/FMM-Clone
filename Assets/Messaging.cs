using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Messaging : MonoBehaviour
{
    public Displaycontroller dc;
    public GameObject messagesPrefab;
    public TMP_Text messageTabText;

    public void LoadSendMessage(string reciever, string[] data, string type, bool viewable, string sender = "")
    {
        if (sender == null || sender == "")
        {
            //if sender isnt assinged assume its from the user
            StartCoroutine(SendMail(DBManager.username, reciever, data, type, viewable));
        }
        else
        {
            //use the actual sender
            StartCoroutine(SendMail(sender, reciever, data, type, viewable));
        }



    }
    IEnumerator SendMail(string sender, string reciever, string[] data, string type, bool viewable)
    {
        //send data to data base
        string datastring = string.Join(",", data);
        WWWForm form = new WWWForm();
        form.AddField("sender", sender);
        form.AddField("reciever", reciever);
        form.AddField("viewable", viewable.ToString());
        form.AddField("data", datastring);
        form.AddField("type", type);
        WWW www = new WWW("192.168.0.20/sqlconnect/createmessage.php", form);
        yield return www;
    }

    void LoadCheckMessages()
    {
        StartCoroutine(CheckNewMessages());
    }

    void LoadDeleteMessage(string[] data)
    {
        
        StartCoroutine(Deletemessage(data));
    }

    IEnumerator Deletemessage(string[] datatxt)
    {
        //Delete message from database
        //TODO do it from ID
        WWWForm form = new WWWForm();
        form.AddField("sender", datatxt[0]);
        form.AddField("reciever", datatxt[1]);
        form.AddField("data", datatxt[2]);
        form.AddField("type", datatxt[3]);
        form.AddField("viewable", datatxt[4]);
        form.AddField("viewed", datatxt[5]);
        WWW www = new WWW("192.168.0.20/sqlconnect/removemessage.php", form);
        yield return www;
    }
    IEnumerator CheckNewMessages()
    {
        //Check for new messages of the user
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        WWW www = new WWW("192.168.0.20/sqlconnect/loadmessages.php", form);
        yield return www;

        string[] webResults = www.text.Split('.');
        foreach (string s in webResults)
        {

            if (s != "")
            {

                string[] sSplit = s.Split('\t');
                //Incase of something breaking uncomment this
                //Debug.Log(sSplit[3]);
                if (sSplit[3] == "requestBattle ")
                {
                    //Battle the cards
                    string[] cards = sSplit[2].Split(',');
                    dc.LoadPvPBattle(cards[0], cards[1], sSplit[0], sSplit[1]);
                    //Delete the message before its called again
                    LoadDeleteMessage(sSplit);
                }
                if (sSplit[3] == "battleResults " && sSplit[1] == DBManager.username)
                {
                    //Show battle results
                    string[] cards = sSplit[2].Split(',');

                    string[] battleresults = sSplit[2].Split(',');

                    dc.messageBoxTexts[0].text = battleresults[0];
                    dc.messageBoxTexts[1].text = battleresults[1];
                    dc.messageBoxTexts[2].text = battleresults[2];
                    //TODO Show opponents username
                    dc.messageBoxTexts[3].text = battleresults[3] + ": Wins";
                    dc.messageBox.SetActive(true);
                    //Delete message
                    LoadDeleteMessage(sSplit);
                }   
                    if (sSplit[5] == "0" && sSplit[4] == "1")
                    {
                        //if not viewed and viewable
                        if (sSplit[1] == DBManager.username)
                        {
                            //add to unread messages
                            if (!dc.unreadMessages.Contains(s))
                            {
                                dc.unreadMessages.Add(s);
                            }
                        }
                        //update the message text
                        ChangeMessageText();
                    }
            }
        }
    }
    public void LoadMessages()
    {
        StartCoroutine(Messages());
    }

    IEnumerator Messages()
    {
        //Load message of user
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        WWW www = new WWW("192.168.0.20/sqlconnect/loadmessages.php", form);
        yield return www;
        string[] webResults = www.text.Split('.');
        //Remove all of previous messages if there
        int childs = dc.msgboxPanel.transform.childCount;
        if (childs != 0)
        {
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(dc.msgboxPanel.transform.GetChild(i).gameObject);

            }
        }
        //open message page
        dc.msgboxPanel.SetActive(true);
        ReadMessages();

        foreach (string s in webResults)
        {
            string[] data = s.Split('\t');

            if (s != "" && data[4] != "0")
            {
                //Load a message if viewable or not null
                GameObject mBox = Instantiate(messagesPrefab, dc.msgboxPanel.transform);


                Button[] btns = mBox.GetComponentsInChildren<Button>();
                //If you arent the reciever but the sender dont show the accept or deny buttons
                if (DBManager.username != data[1])
                {
                    btns[0].gameObject.SetActive(false);
                    btns[1].gameObject.SetActive(false);
                }
                //if clicked deny or accept
                btns[0].onClick.AddListener(delegate { AcceptMessage(s); });
                btns[1].onClick.AddListener(delegate { DenyMessage(s); });
                TMP_Text[] txts = mBox.GetComponentsInChildren<TMP_Text>();
                TMP_Text msgtxt;

                foreach (TMP_Text t in txts)
                {
                    if (t.gameObject.name == "Message text")
                    {
                        msgtxt = t;
                        //TODO setup customtext
                        msgtxt.text = s;
                    }
                }


            }
        }
    }

    void AcceptMessage(string data)
    {
        //Delete message then reload
        DenyMessage(data);
        LoadMessages();
        //TODO Check if whats recieved is a battlerequest
        LoadMessageCheck(data);

    }

    void LoadMessageCheck(string data)
    {
        //Send a requestbattle to the other person
        string[] datatxt = data.Split('\t');
        string[] senddata = datatxt[2].Split(',');
        LoadSendMessage(datatxt[1], senddata, "requestBattle", false, datatxt[0]);



    }

    void DenyMessage(string data)
    {
        //just delete the message and reload messages
        string[] dataarray = data.Split('\t');
        LoadDeleteMessage(dataarray);
        LoadMessages();
        ChangeMessageText();

    }

    IEnumerator ReadMessages()
    {
        //Set all messages in inbox to read
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        WWW www = new WWW("192.168.0.20/sqlconnect/readmessages.php", form);

        yield return www;
    }

    public void ChangeMessageText()
    {
        //if no new messages set it to normal else show the unread messages
        if (dc.unreadMessages.Count == 0)
        {
            messageTabText.text = "Messages";
        }
        else
        {
            messageTabText.text = "Messages: " + dc.unreadMessages.Count.ToString();
        }

    }
}
