using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//!only attached to +1 pot size
public class IncreaseSliderPotSize : MonoBehaviour {

	Slider theSlider;

	// Use this for initialization
	void Start () {
		GameObject temp = GameObject.Find ("Bet Slider");
		if (temp != null) {
			theSlider = temp.GetComponent<Slider> ();
		} else {
			Debug.Log ("temp is null in +1Pot Size");
		}
	}

	//this function in onclick
	public void potSizeIncreaseClicked(){
		theSlider.value = theSlider.value + (float)GlobalVars.Pot;
	}
}
