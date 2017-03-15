using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//!only attached to -1BB
public class DecreaseSliderBB : MonoBehaviour {
	Slider theSlider;

	// Use this for initialization
	void Start () {
		GameObject temp = GameObject.FindGameObjectWithTag ("Player");
		if (temp != null) {
			theSlider = temp.GetComponent<Slider> ();
		} else {
			Debug.Log ("temp is null in -1BB");
		}
	}

	//this function in onclick
	public void bigBlindDecreaseClicked(){
		theSlider.value = theSlider.value - (float)GlobalVars.BigBlind;
	}
}
