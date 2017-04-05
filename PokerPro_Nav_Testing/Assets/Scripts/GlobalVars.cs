using UnityEngine;
using System.Collections;

//!only attached to the canvas.

//5k/10k stakes w/ 1m buyin
public class GlobalVars : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this);// maintain globals vars
    }

    // matchmaking vars
    public static string player_id = null;
    public static string game_id = null;

    public const int BigBlind = 10000;
	//http get bigBlindSize

	//public const int SmallBlind = ;
	//http get smallblindsiz

	public static int Pot = 5000; //placeholder for the Pot Size Bet button
	//http get currPotSize

	public static int bet = 0;// this players bet
    public static int curr_bet = 0;// the bet currently on the table

	//bug when this value is large (like 2m instead of 1m), slider will jump by more than 10k at a time. Can hopefully fix with scaling related to chipstack size.
	//bug isn't functionally hurtful but needs fixing before release
	public static int chipStack = 1000000; 
}