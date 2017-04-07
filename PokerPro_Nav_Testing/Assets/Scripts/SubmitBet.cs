using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using LitJson;

//!only attached to the submit button
public class SubmitBet : MonoBehaviour {

	Slider theSlider;

	void Awake () {
		GameObject temp = GameObject.Find("Bet Slider");
		if (temp != null) {
			theSlider = temp.GetComponent<Slider> ();
		} else {
			Debug.Log ("Can't locate the slider.");
		}
	}

    // use this function in on click
    public void sendTheBet() {
        // sends the current bet on the slider
        StartCoroutine(betHelper(false));
    }

    // sumbit a bet equal to the bet currently before you
    public void call()
    {
        // check if the player has enough chips
        // TODO replace with an all in
        if(GlobalVars.curr_bet > GlobalVars.chips)
        {
            Debug.Log("Not enough chips to call.");
            Display_Message.print_message("Not enough chips to call");
        }
        // valid call
        else
        {
            turnOffSlider();
            GlobalVars.bet = GlobalVars.curr_bet;
            StartCoroutine(betHelper(false));// run bet helper like normal
        }   
    }

    // turns off the slider so that the bet can no longer be modified
    private void turnOffSlider()
    {
        UpdateBet.disableSlider();
    }

    // turns on the slider so that the bet can be modified
    private void turnOnSlider()
    {
        UpdateBet.enableSlider();
    }

    // submit a bet of all your chips
    // TODO handle the case where all in does not cover the call, currently it does not allow this
    public void allIn()
    {
        turnOffSlider();
        GlobalVars.bet = GlobalVars.chips;
        StartCoroutine(betHelper(false));// run bet helper like normal
    }

    // submit a fold
    public void fold()
    {
        turnOffSlider();
        GlobalVars.bet = 0;// set the bet to 0 
        StartCoroutine(betHelper(true));// run bet helper like normal
    }

    protected IEnumerator betHelper(bool isFolded) {
        // check that the bet is valid - must be at least as large as the bet needed to call
        if (GlobalVars.bet < GlobalVars.curr_bet && !isFolded)
        {
            Debug.Log("Invalid bet. Your bet: " + GlobalVars.bet + "\nBet Needed: " + GlobalVars.curr_bet);
            Display_Message.print_message("Submit a bet over " + GlobalVars.curr_bet + " or fold.");
            turnOnSlider();// renable if disabled by all in
        }

        // bet is valid or a fold - submit the bet and get the gamestate in return
        else
        {
            if (isFolded)
                Debug.Log("Sending Fold...");
            else 
                Debug.Log("Sending bet...");

            turnOffSlider();// disbale the bet slider

            // send the bet to the server
            string url = "http://104.131.99.193/game/" + GlobalVars.game_id + "/" + GlobalVars.player_id + "/" + GlobalVars.bet;
            WWW www = new WWW(url);
            yield return www;

            // error check
            if (www.error != null)
            {
                Debug.Log("WWW submit bet error: " + www.error);
            }
            else
            {
                Debug.Log("Bet of " + GlobalVars.bet + " sent.");
                if (GlobalVars.bet == 0 && !isFolded)
                    Display_Message.print_message("You have checked.");
                else if(isFolded)
                    Display_Message.print_message("You have folded.");
                else
                    Display_Message.print_message("You have bet " + GlobalVars.bet + ".");

                // update the gamestate
                Debug.Log("Sent bet and got a gamestate.");
                string jsonString = www.text;
                var gameStateJson = JsonMapper.ToObject(jsonString);
                DebugChangeCard.gameState = gameStateJson;
                DebugChangeCard.gameGlobals.numGamePlayers = gameStateJson["players"].Count;
                DebugChangeCard.gameGlobals.me = (int)gameStateJson["me"];
                DebugChangeCard.gameGlobals.isLoaded = true;
                Debug.Log("Pot: " + gameStateJson["pot"]);// delete this
                GlobalVars.Pot = (int)gameStateJson["pot"];

                // reset the bet needed to call
                GlobalVars.curr_bet = 0;

                // disable the bet UI
                Disable_Buttons.disableButtons();
                theSlider.value = 0;// also makes GlobalVars.bet=0
                StartCoroutine(DebugChangeCard.getUpdatedGameState());
            }
        }
	}
}
