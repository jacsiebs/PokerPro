using UnityEngine;
using System.Collections;
using LitJson;

//!only attached to the canvas.

//5k/10k stakes w/ 1m buyin
public class FakeGlobalVars
{
    // matchmaking vars
    public  string player_id;
    public  string game_id = null;

    // game state holder
    public JsonData gameState = JsonMapper.ToObject("{\"tournamentId\":\"1\",\"buyin\":1000000,\"game\":1,\"hand\":1,\"spinCount\":0,\"sb\":0,\"pot\":0,\"sidepots\":[],\"commonCards\":[],\"db\":0,\"callAmount\":0,\"minimumRaiseAmount\":0,\"players\":[{\"id\":\"20\",\"name\":\"xXP0k3RsLaY3rXx\",\"status\":\"active\",\"chips\":1000000,\"chipsBet\":0},{\"id\":\"30\",\"name\":\"xXP0k3RsLaY3rXx\",\"status\":\"active\",\"chips\":1000000,\"chipsBet\":0,\"cards\":[{\"rank\":\"2\",\"type\":\"C\"},{\"rank\":\"K\",\"type\":\"S\"}]}],\"me\":1,\"currentPlayer\":\"20\"}");

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
