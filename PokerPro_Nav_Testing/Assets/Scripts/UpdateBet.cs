using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//!only attached to the slider
public class UpdateBet : MonoBehaviour {

	public static Slider TheBetSlider;

	// Use this for initialization
	void Awake () {
		TheBetSlider = gameObject.GetComponentInParent<Slider> ();
		TheBetSlider.maxValue = GlobalVars.chips;
	}

    // disable the bet slider
    public static void disableSlider()
    {
        TheBetSlider.interactable = false;
    }

    // enable the bet slider
    public static void enableSlider()
    {
        TheBetSlider.interactable = true;
    }

    //this function in On Value Changed
    public void sliderValueChanged(){
		if (TheBetSlider.value == TheBetSlider.maxValue) {
			GlobalVars.bet = GlobalVars.chips;
		} else {
			//bug when chipstack is large (like 2m instead of 1m), slider will jump by more than 10k at a time. Can hopefully fix with scaling related to chipstack size.
			//bug isn't functionally hurtful but needs fixing before release
			GlobalVars.bet = ((int)TheBetSlider.value/10000)*10000; //bug when chipstack is large, this increase by larger multiples of 10k e.g. 40k at a time. can possible scaling fix.
		}

		//Debug.Log (GlobalVars.bet);
	}
}
