using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hellomessage : MonoBehaviour
{
    public TMP_Text textBox;

    private void Start()
    {
        textBox.text = "Welcome " + DBManager.username;
    }
}
