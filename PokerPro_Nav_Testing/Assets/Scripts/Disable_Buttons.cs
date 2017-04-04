using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Disable_Buttons : MonoBehaviour {
    static GameObject all_in;
    static GameObject fold;
    static GameObject call;
    static GameObject bet;
    
	// Use this for initialization
	void Start () {
        all_in = GameObject.Find("All_In");
        fold = GameObject.Find("Fold");
        call = GameObject.Find("Call");
        bet = GameObject.Find("Place Bet");
    }
	
    // greys out all buttons relating to betting - used when it is not the player's turn
	public static void disableButtons()
    {
        all_in.GetComponent<Button>().interactable = false;
        fold.GetComponent<Button>().interactable = false;
        call.GetComponent<Button>().interactable = false;
        bet.GetComponent<Button>().interactable = false;
    }

    // enables all buttons - for the players turn to bet
    public static void enableButtons()
    {
        all_in.GetComponent<Button>().interactable = true;
        fold.GetComponent<Button>().interactable = true;
        call.GetComponent<Button>().interactable = true;
        bet.GetComponent<Button>().interactable = true;
    }


}
