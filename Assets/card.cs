using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class card : ScriptableObject
{
    public string cardName;

    public Image cardImage;

    public int attack;
    public int defense;
    public int intelligence;
    public int skills;

}
