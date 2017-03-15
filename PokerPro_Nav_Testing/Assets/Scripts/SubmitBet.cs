using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//!only attached to the submit button
public class SubmitBet : MonoBehaviour {

	Slider theSlider;

	void Start () {
		GameObject temp = GameObject.FindGameObjectWithTag ("Player");
		if (temp != null) {
			theSlider = temp.GetComponent<Slider> ();
		} else {
			Debug.Log ("temp is null in submit bet");
		}
	}

	//this function in onclick
	public void sendTheBet(){
		Debug.Log ("Submit Clicked");
		//http.submitBet
		Debug.Log("Slider/bet value before submit:");
		Debug.Log(GlobalVars.bet);
		//http.post("bet")
		theSlider.value = 0;
		Debug.Log("Slider/bet value after submit:");
		Debug.Log(GlobalVars.bet);
	}
}
