using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//!only attached to the +1BB button
public class IncreaseSliderBB : MonoBehaviour {
	Slider theSlider;

	// Use this for initialization
	void Start () {
		GameObject temp = GameObject.Find ("Bet Slider");
		if (temp != null) {
			theSlider = temp.GetComponent<Slider> ();
		} else {
			Debug.Log ("temp is null in +1BB");
		}
	}

	//this function in onclick
	public void bigBlindIncreaseClicked(){
		theSlider.value = theSlider.value + (float)GlobalVars.BigBlind;
	}
}
