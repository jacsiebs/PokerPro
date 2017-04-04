using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display_Bet : MonoBehaviour {

    public Text Bet_Display;
    private GameObject thisObject;

	// Use this for initialization
	void Start () {
        thisObject = GameObject.Find("Bet_Display_Script");
        Bet_Display = thisObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        Bet_Display.text = GlobalVars.bet.ToString();
	}
}
