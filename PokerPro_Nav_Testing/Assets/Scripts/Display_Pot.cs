using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display_Pot : MonoBehaviour
{

    public Text Pot_Display;
    private GameObject thisObject;

    // Use this for initialization
    void Start()
    {
        thisObject = GameObject.Find("Pot_Display_Script");
        Pot_Display = thisObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Pot_Display.text = GlobalVars.Pot.ToString();
    }
}
