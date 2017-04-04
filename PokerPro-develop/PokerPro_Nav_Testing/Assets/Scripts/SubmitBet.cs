using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

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
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.Send();

        // error check
        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            theSlider.value = 0;
            Debug.Log("Bet sent");
        }
	}
}
