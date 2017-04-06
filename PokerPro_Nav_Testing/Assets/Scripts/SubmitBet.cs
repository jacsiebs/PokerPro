using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using LitJson;

//!only attached to the submit button
public class SubmitBet : MonoBehaviour {

	Slider theSlider;

	void Start () {
		GameObject temp = GameObject.Find("Bet Slider");
		if (temp != null) {
			theSlider = temp.GetComponent<Slider> ();
		} else {
			Debug.Log ("temp is null in submit bet");
		}
	}

    // use this function in on click
    public void sendTheBet() {
        StartCoroutine(betHelper());
    }
    protected IEnumerator betHelper() {
		Debug.Log("Sending bet...");
        // send the bet to the server
        string url = "http://104.131.99.193/game/" + GlobalVars.game_id + "/" + GlobalVars.player_id + "/" + GlobalVars.bet;
        WWW www = new WWW(url);
        yield return www;
        // update the gamestate
        Debug.Log("Got a gamestate.");
        string jsonString = www.text;
        var gameStateJson = JsonMapper.ToObject(jsonString);
        DebugChangeCard.gameState = gameStateJson;
        DebugChangeCard.gameGlobals.numGamePlayers = gameStateJson["players"].Count;
        DebugChangeCard.gameGlobals.me = (int)gameStateJson["me"];
        DebugChangeCard.gameGlobals.isLoaded = true;
        Debug.Log("Pot: " + gameStateJson["pot"]);// delete this
        GlobalVars.Pot = (int)gameStateJson["pot"];

        // error check
        if (www.error != null)
        {
            Debug.Log("WWW submit bet error: " + www.error);
        }
        else
		{
			Disable_Buttons.disableButtons();
            theSlider.value = 0;
            Debug.Log("Bet sent");
			StartCoroutine(DebugChangeCard.getUpdatedGameState());
        }
	}
}
