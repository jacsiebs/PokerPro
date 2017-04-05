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
    public static string player_id = "47";
    public static string game_id = null;

    public const int BigBlind = 10000;
	//http get bigBlindSize

	//public const int SmallBlind = ;
	//http get smallblindsiz

	public static int Pot = 0; //current pot on the table
	//http get currPotSize

	public static int bet = 0;//initialization

	//bug when this value is large (like 2m instead of 1m), slider will jump by more than 10k at a time. Can hopefully fix with scaling related to chipstack size.
	//bug isn't functionally hurtful but needs fixing before release
	public static int chipStack = 1000000; 
}