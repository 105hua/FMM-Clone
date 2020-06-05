using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net.WebSockets;
using System.Globalization;
using System.Linq;

public class Displaycontroller : MonoBehaviour
{
    public profilesearch psearch;
    public Messaging msg;
    public List<string> unreadMessages = new List<string>();

    public TMP_Text cardValue;
    public TMP_Text cardPrice;
    public GameObject cardPrefab;
    public GameObject messagesPrefab;


    public GameObject[] cards;
    

    public TMP_Text messageTabText;

    public GameObject msgboxPanel;

    public TMP_Text sidebarText;

    public GameObject[] bodyPanels;
    public GameObject bodyViewport;
    public TMP_Text[] bodyText;

    public Vector3 cardMenuOffset;
    public GameObject cardMenu;
    public GameObject[] cardMenuButtons;
    public GameObject shopBody;
    public GameObject shopPanel;

    public GameObject MainUI;
    public GameObject BattleUI;
    public GameObject DojoPanel;

    public GameObject DojoFightButton;

    public GameObject messageBox;

    public TMP_Text[] messageBoxTexts;

    public GameObject selectCardFight;

    string reciever;
    string enemycard = "";

    

    public IEnumerator LoadUserData(string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("searchname", username);
        WWW www = new WWW("192.168.0.20/sqlconnect/getuserdata.php", form);
        yield return www;
        string[] webResults = www.text.Split('\t');
        
        //Display the users cards
        DisplayData(webResults);
        LoadDisplayCards(webResults[0], bodyViewport.transform);
    }

    void DisplayData(string[] data)
    {
        //adds top text saying name and currency
        bodyText[0].text = data[0];
        bodyText[1].text = data[1];
        //opens the body panels
        bodyPanels[0].gameObject.SetActive(true);
        bodyPanels[1].gameObject.SetActive(true);
    }

    void LoadCreateCard (int level)
    {
        StartCoroutine(CreateCard(level));
    }
    //Creates card at level 1
    //TODO add more levels
    IEnumerator CreateCard(int level)
    {
        WWWForm form = new WWWForm();
        form.AddField("owner", DBManager.username);
        form.AddField("level", level);
        WWW www = new WWW("192.168.0.20/sqlconnect/createcard.php", form);
        yield return www;
        StartCoroutine(LoadUserData(DBManager.username));


    }

    void LoadDisplayCards(string user, Transform panel, bool showMenu = true)
    {
        StartCoroutine(DisplayCards(user, panel.transform, showMenu));
    }

    IEnumerator DisplayCards(string user, Transform panel, bool showMenu)
    {
        //Gets cards of user
        WWWForm form = new WWWForm();
        form.AddField("user", user);
        WWW www = new WWW("192.168.0.20/sqlconnect/loadcards.php", form);
        yield return www;
        string[] robotsarray = www.text.Split(',');
        int childs = panel.childCount;
        
        for (int i = childs - 1; i >= 0; i--)
        {
            //readies up the cards array and clears the cards
            GameObject.DestroyImmediate(panel.GetChild(i).gameObject);
            cards = null;
        }
        foreach (string s in robotsarray)
        {
            GameObject btn = Instantiate(cardPrefab, panel);
            TMP_Text btntxt = btn.GetComponentInChildren<TMP_Text>();
            if (cards == null)
            {
                if (s != "")
                {
                    //each card opens the menu
                    btntxt.text = s;
                    Button b = btn.GetComponent<Button>();
                    b.onClick.AddListener(delegate { LoadCardMenu(s, user, showMenu); });
                }
                else
                {
                    if (user == DBManager.username && showMenu)
                    {
                        //if its on the user show the new monster card
                        btntxt.text = "New monster";
                        Button b = btn.GetComponent<Button>();
                        b.onClick.AddListener(delegate { LoadCreateCard(1); });
                    }
                    else
                    {
                        //destroy empty button
                        Destroy(btn.gameObject);
                    }    
                }
            }
        }
    }

    public void LoadAddCardToMarket(string cardName)
    {
        StartCoroutine(AddCardToMarket(cardName));
    }

    IEnumerator AddCardToMarket(string cardName)
    {
        int price = 0;
        int.TryParse(cardValue.text, out price);
        WWWForm form = new WWWForm();
        form.AddField("owner", DBManager.username);
        form.AddField("value", price);
        form.AddField("card", cardName);
        WWW www = new WWW("192.168.0.20/sqlconnect/addcardtomarket.php", form);
        yield return www;
        //creates a new card
        if (www.text == "0")
        {
            CloseCardMenu();
            LoadDisplayCards(DBManager.username, bodyViewport.transform);
            //if successfull reload the cards
        }
        else
        {
            //if fucks up cry
            Debug.LogError(www.text);
        }
    }

    public void LoadCardMenu(string card, string username, bool showMenu)
    {
        //kill all children
        int childs = cardMenu.transform.childCount;

        for (int i = childs - 1; i >= 0; i--)
        {
            if (cardMenu.transform.GetChild(i).gameObject.tag == "pButton")
            {
                GameObject.DestroyImmediate(cardMenu.transform.GetChild(i).gameObject);
            } 
                
           
            
        }
        //close each button
        //TODO automate this
        cardMenuButtons[2].SetActive(false);
        cardMenuButtons[1].SetActive(false);
        cardMenuButtons[0].SetActive(false);
        cardMenuButtons[3].SetActive(false);
        cardMenuButtons[4].SetActive(false);
        selectCardFight.SetActive(false);

        if (username == DBManager.username && showMenu)
        {
            
            cardMenuButtons[2].SetActive(true);
            cardMenuButtons[1].SetActive(false);
            cardMenuButtons[0].SetActive(true);
            cardMenuButtons[4].SetActive(true);
            cardPrice.gameObject.SetActive(false);
            string[] cardInfo = card.Split('\t');
            cardMenu.SetActive(true);
            Button marketbutton = cardMenuButtons[0].GetComponent<Button>();
            marketbutton.onClick.AddListener(delegate { LoadAddCardToMarket(cardInfo[0]); });
            //market button and points to addcardtomarket

            GameObject btn = Instantiate(cardPrefab, cardMenu.transform.position + cardMenuOffset, cardMenu.transform.rotation, cardMenu.transform);
            TMP_Text btntxt = btn.GetComponentInChildren<TMP_Text>();
            btntxt.text = card;
            Button b = btn.GetComponent<Button>();
            b.interactable = false;
            //adds the robot card

            Button dojobutton = cardMenuButtons[2].GetComponent<Button>();
            dojobutton.onClick.AddListener(delegate { SendCardToDojo(cardInfo[0]); });
            GameObject dojobtn = Instantiate(cardPrefab, DojoPanel.transform.position + cardMenuOffset, DojoPanel.transform.rotation, DojoPanel.transform);
            TMP_Text dojobtntxt = dojobtn.GetComponentInChildren<TMP_Text>();
            dojobtntxt.text = card;
            Button dojob = dojobtn.GetComponent<Button>();
            dojob.interactable = false;
            //adds the dojo button and gets the name of the card

        }
        else if (username == "market" && showMenu)
        {
            //if its selected from the marked show the buy option
            cardMenuButtons[2].SetActive(false);
            cardMenuButtons[0].SetActive(false);
            cardMenuButtons[4].SetActive(false);
            cardMenuButtons[1].SetActive(true);
            cardPrice.gameObject.SetActive(true);

            string[] cardInfo = card.Split('\t');
            Debug.Log(cardInfo[0]);
            cardMenu.SetActive(true);
            Button marketbutton = cardMenuButtons[1].GetComponent<Button>();
            marketbutton.onClick.AddListener(delegate { LoadBuyCardFromMarket(cardInfo[0]); });

            GameObject btn = Instantiate(cardPrefab, cardMenu.transform.position + cardMenuOffset, cardMenu.transform.rotation, cardMenu.transform);
            TMP_Text btntxt = btn.GetComponentInChildren<TMP_Text>();
            btntxt.text = card;
            Button b = btn.GetComponent<Button>();
            b.interactable = false; 
        }        
        else
        {

            if (!showMenu)
            {
                List<string> totaldata = new List<string>();
                //Select card
                selectCardFight.SetActive(false);
                string[] cardInfo = card.Split('\t');
                
                totaldata.Add(cardInfo[0]);
                totaldata.Add(enemycard);
                
                string[] dataarray = totaldata.ToArray();
                msg.LoadSendMessage(reciever, dataarray, "battle", true);
                Debug.Log(reciever + "\t" + cardInfo[0] + "\t" + "battle" +"\t" + false);
               


            } else
            {
                //show own card and get enemy card
                Debug.Log(showMenu);
                cardMenu.SetActive(true);
                string[] cardInfo = card.Split('\t');
                enemycard = cardInfo[0];
                GameObject btn = Instantiate(cardPrefab, cardMenu.transform.position + cardMenuOffset, cardMenu.transform.rotation, cardMenu.transform);
                TMP_Text btntxt = btn.GetComponentInChildren<TMP_Text>();
                btntxt.text = card;
                Button b = btn.GetComponent<Button>();
                b.interactable = false;
                //when click fight button select own card
                Button fightbutton = cardMenuButtons[3].GetComponent<Button>();
                fightbutton.onClick.AddListener(delegate { OpenSelectOwnCard(); });
                cardMenuButtons[3].SetActive(true);
                reciever = username;
                
            }
            
        }
        
    }

    void OpenSelectOwnCard()
    {
        //display all of the users cards
        LoadDisplayCards(DBManager.username, selectCardFight.transform, false);
        selectCardFight.SetActive(true);
        //close the previous card menu
        CloseCardMenu();
    }
    void SendCardToDojo(string card)
    {
        //Open the dojo and show the card to train
        OpenDojo();
        Button fightbtn = DojoFightButton.GetComponent<Button>();
        fightbtn.onClick.AddListener(delegate { LoadDojoBattle(card); });
    }

    void LoadBuyCardFromMarket(string cardName)
    {
        Debug.Log("1");
        StartCoroutine(BuyCardFromMarket(cardName));
    }

    IEnumerator BuyCardFromMarket(string cardName)
    {
        //Buy card from market
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("card", cardName);
        WWW www = new WWW("192.168.0.20/sqlconnect/buycard.php", form);
        yield return www;
        //Open back onto market
        CloseCardMenu();
        LoadDisplayCards("market", shopPanel.transform);
        
        
    }


    public void CloseCardMenu()
    {
        //closes card menu
        cardMenu.SetActive(false); 
    }

    public void OpenShop()
    {
        //close all other panels
        //TODO FUCKING AUTOMATE THIS
        selectCardFight.SetActive(false);
        msgboxPanel.SetActive(false);
        shopBody.gameObject.SetActive(true);
        bodyPanels[0].gameObject.SetActive(false);
        bodyPanels[1].gameObject.SetActive(false);
        psearch.profilePanel.SetActive(false);
        DojoPanel.SetActive(false);
        //Display cards that are in the market
        LoadDisplayCards("market", shopPanel.transform);
    }
    public void OpenDojo()
    {
        //opens the dojo
        //TODO CAN I AUTOMATE THIS ALREADY
        msgboxPanel.SetActive(false);
        shopBody.gameObject.SetActive(false);
        bodyPanels[0].gameObject.SetActive(false);
        bodyPanels[1].gameObject.SetActive(false);
        psearch.profilePanel.SetActive(false);
        DojoPanel.SetActive(true);
        cardMenu.SetActive(false);
    }
    public void CloseDojo()
    {
        //Open back into home
        DojoPanel.SetActive(false);
        bodyPanels[0].gameObject.SetActive(true);
        bodyPanels[1].gameObject.SetActive(true);

        //Kill all children
        int childs = DojoPanel.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            if (DojoPanel.transform.GetChild(i).gameObject.tag == "pButton")
            {
                GameObject.DestroyImmediate(DojoPanel.transform.GetChild(i).gameObject);
            }

        }
    }

    public void LoadDojoBattle(string card)
    {
        StartCoroutine(DojoBattle(card));
    }

    IEnumerator DojoBattle(string card)
    {
        //Start Dojo battle
        WWWForm form = new WWWForm();
        form.AddField("card1", card);
        WWW www = new WWW("192.168.0.20/sqlconnect/battle.php", form);
        yield return www;
        string[] webResults = www.text.Split('.');
        
        //Display results
        //TODO Automate the message box
        messageBoxTexts[0].text = webResults[0];
        messageBoxTexts[1].text = webResults[1];
        messageBoxTexts[2].text = webResults[2];
        messageBoxTexts[3].text = webResults[3] + ": Wins";
        messageBox.SetActive(true);
        
    }

    public void CloseMessageBox()
    {
        //closes message box
        messageBox.SetActive(false);
        messageBoxTexts[0].text = "";
        messageBoxTexts[1].text = "";
        messageBoxTexts[2].text = "";
        messageBoxTexts[3].text = "";
    }

   

    public void LoadPvPBattle(string card1 ,string card2, string user1,string user2)
    {
        StartCoroutine(PvPBattle(card1, card2, user1, user2));
    }

    IEnumerator PvPBattle(string card1, string card2, string user1, string user2)
    {
        //Start fight between both cards
        WWWForm form = new WWWForm();
        form.AddField("card1", card1);
        form.AddField("card2", card2);
        form.AddField("type", "battle");
        WWW www = new WWW("192.168.0.20/sqlconnect/battle.php", form);
        yield return www;
        string[] webResults = www.text.Split('.');

        //Opens message box
        messageBoxTexts[0].text = webResults[0];
        messageBoxTexts[1].text = webResults[1];
        messageBoxTexts[2].text = webResults[2];
        //TODO Show username of winner
        messageBoxTexts[3].text = webResults[3] + ": Wins";
        messageBox.SetActive(true);
        //Send the results to the other person
        msg.LoadSendMessage(user2, webResults, "battleResults", false, DBManager.username);
        

    }

    
}
