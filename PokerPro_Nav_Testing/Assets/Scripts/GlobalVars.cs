using UnityEngine;
using System.Collections;

//!only attached to the canvas.

//5k/10k stakes w/ 1m buyin
public class GlobalVars : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(this);// maintain globals vars
		player_id = Random.Range(0, 9999999).ToString();// TODO placeholder to get a random player id
    }

    // player avatar
    public static Sprite square_avatar = null;// a facebook profile picture or premade avatar - no cropping
    public static Sprite avatar = null;// a facebook profile picture or premade avatar, cropped to a circle
    public static int avatar_num = 1;// TODO remove placeholder - loaded in from server
    public static Sprite rank_sprite = null;
    public static int ELO;
    public static string rank;

    // matchmaking vars
    public static string fb_id;
	public static string player_id = null;
    public static string game_id;

    // currently unused - placeholders
    public const int BigBlind = 10000;
	public const int SmallBlind = 5000;

	public static int Pot = 0; //current pot on the table
    public static int oldPot = 0;// the pot size of the last hand, used to calculate the current bet
    public static int chipsBet = 0;// number of chips put into the current pot by the player
	public static int bet = 0;// curent bot
    public static int curr_bet = 0;// current bet on the table

	//bug when this value is large (like 2m instead of 1m), slider will jump by more than 10k at a time. Can hopefully fix with scaling related to chipstack size.
	//bug isn't functionally hurtful but needs fixing before release
	public static int init_chips = 1000000;// starting number of chips per player
    public static int chips = init_chips;// the number of chips the player has
}