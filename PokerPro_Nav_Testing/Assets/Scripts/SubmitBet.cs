using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

//!only attached to the submit button
public class SubmitBet : MonoBehaviour {

	Slider theSlider;
    GameObject disable_script;

	void Start () {
		GameObject temp = GameObject.Find("Bet Slider");
		if (temp != null) {
			theSlider = temp.GetComponent<Slider> ();
		} else {
			Debug.Log ("Cannot find the bet slider object.");
		}
        disable_script = GameObject.Find("Enable_Disable_Buttons");
	}

    // use this function in on click
    public void sendTheBet() {
        StartCoroutine(betHelper());
    }

    // sends a get request to the server containing the bet which the player has made
    protected IEnumerator betHelper() {
		Debug.Log("Sending bet...");
        // send the bet to the server
        string url = "http://104.131.99.193/game/" + GlobalVars.game_id + "/" + GlobalVars.player_id + "/" + GlobalVars.bet;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.Send();

        // error check
        if (www.isError)
        {
            Debug.Log("Error in sending bet: " + www.error);
        }
        else
        {
            Debug.Log("Bet of " + GlobalVars.bet + " sent.");
            theSlider.value = 0;// this also changes GlobalVars.bet to 0
            // buttons are disabled once the new gamestate is received 
        }
	}
}
