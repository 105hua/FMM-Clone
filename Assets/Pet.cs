using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

[CreateAssetMenu]
public class Pet : ScriptableObject
{
    public string Name;

    public int Attack;
    public int Defense;
    public int Intelligence;
    public int Skill;

    public bool isBot;

    public int level;

}
