using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display_Curr_Bet : MonoBehaviour
{

    public Text Curr_Bet_Display;
    private GameObject thisObject;

    // Use this for initialization
    void Awake()
    {
        thisObject = GameObject.Find("Current_Bet_Display_Script");
        Curr_Bet_Display = thisObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Curr_Bet_Display.text = "Bet on Table: " + GlobalVars.curr_bet.ToString();
    }
}