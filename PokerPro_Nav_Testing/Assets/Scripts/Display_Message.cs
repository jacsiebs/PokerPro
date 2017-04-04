using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display_Message : MonoBehaviour
{

    public Text Message_Display;
    private GameObject thisObject;
    public string Message;

// Use this for initialization
void Start()
{
    thisObject = GameObject.Find("Message_Display_Script");
    Message_Display = thisObject.GetComponent<Text>();
    Message = "";
}

// Update is called once per frame
void Update()
{
    Message_Display.text = Message;
}
}

