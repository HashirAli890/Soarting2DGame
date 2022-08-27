using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppAttriutes : MonoBehaviour
{
    public string ColorName;
    public string AppCatagory;
    public string Creator;
    public string AppName;
    public Text nameText;

   
    public void ShowText()
    {
        nameText.text = AppName;
    }
}
