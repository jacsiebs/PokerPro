using UnityEngine;
using System.Collections;

//!only attached to the canvas.

//5k/10k stakes w/ 1m buyin
public class FakeGlobalVars
{
    // matchmaking vars
    public  string player_id;
    public  string game_id = null;

    // currently unused - placeholders
    public const int BigBlind = 10000;
    public const int SmallBlind = 5000;

    public int Pot = 0; //current pot on the table
    public int oldPot = 0;// the pot size of the last hand, used to calculate the current bet
    public int chipsBet = 0;// number of chips put into the current pot by the player
    public int bet = 0;// curent bot
    public int curr_bet = 0;// current bet on the table

    //bug when this value is large (like 2m instead of 1m), slider will jump by more than 10k at a time. Can hopefully fix with scaling related to chipstack size.
    //bug isn't functionally hurtful but needs fixing before release
    public int chips = 100000;// the number of chips the player has
}
