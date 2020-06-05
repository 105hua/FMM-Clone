﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    public static string username;

    public static bool loggedIn { get { return username != null; } }

    public static void Logout()
    {
        username = null;
    }
}