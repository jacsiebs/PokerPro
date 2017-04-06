using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display_Message : MonoBehaviour
{

    private static Text Message_Display;
    private static GameObject thisObject;
    public static string Message;

    // Use this for initialization
    void Awake()
    {
        thisObject = GameObject.Find("Message_Display_Script");
        Message_Display = thisObject.GetComponent<Text>();
        Message = "";
    }

    // print string m to the message panel
    public static void print_message(string m)
    {
        Message = m;
        Message_Display.text = Message;
    }

    // clear the message panel
    public static void clear()
    {
        Message = "";
        Message_Display.text = Message;
    }
}

