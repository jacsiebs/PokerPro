using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display_Curr_Bet : MonoBehaviour
{

    public Text Curr_Bet_Display;
    private GameObject thisObject;

    // player name displays
    private static Text name_0;
    private static Text name_1;
    private static Text name_2;
    private static Text name_3;
    private static Text name_4;
    private static Text name_5;
    private static Text name_6;
    private static Text name_7;

    // chip count displays
    private static Text chips_0;
    private static Text chips_1;
    private static Text chips_2;
    private static Text chips_3;
    private static Text chips_4;
    private static Text chips_5;
    private static Text chips_6;
    private static Text chips_7;


    // Use this for initialization
    void Awake()
    {
        Debug.Log("Awaken: Display_Curr_Bet");
        thisObject = GameObject.Find("Current_Bet_Display_Script");
        Curr_Bet_Display = thisObject.GetComponent<Text>();
        load_player_displays();
    }

    // Update is called once per frame
    void Update()
    {
        Curr_Bet_Display.text = "Bet on Table: " + GlobalVars.curr_bet.ToString();
    }

    // load the text gameobjects for player info
    public static void load_player_displays()
    {
        name_0 = GameObject.Find("name0").GetComponent<Text>();
        name_1 = GameObject.Find("name1").GetComponent<Text>();
        name_2 = GameObject.Find("name2").GetComponent<Text>();
        name_3 = GameObject.Find("name3").GetComponent<Text>();
        name_4 = GameObject.Find("name4").GetComponent<Text>();
        name_5 = GameObject.Find("name5").GetComponent<Text>();
        name_6 = GameObject.Find("name6").GetComponent<Text>();
        name_7 = GameObject.Find("name7").GetComponent<Text>();

        chips_0 = GameObject.Find("chips0").GetComponent<Text>();
        chips_1 = GameObject.Find("chips1").GetComponent<Text>();
        chips_2 = GameObject.Find("chips2").GetComponent<Text>();
        chips_3 = GameObject.Find("chips3").GetComponent<Text>();
        chips_4 = GameObject.Find("chips4").GetComponent<Text>();
        chips_5 = GameObject.Find("chips5").GetComponent<Text>();
        chips_6 = GameObject.Find("chips6").GetComponent<Text>();
        chips_7 = GameObject.Find("chips7").GetComponent<Text>();
    }


    /* Important Note:
     * Currently these algorithms assume that players always occupy the lowest indicies possible
     */
     
    // used to initalize the player name displays
    // load_player_displays must be called first
    public static void update_player_names(string json)
    {
        Debug.Log("Updating player names");
        var gamestate = JsonMapper.ToObject(json);

        // get the player name and apply it to the correct text display
        for (int i = 0; i < DebugChangeCard.gameGlobals.numGamePlayers; ++i)
        {
            
            string name = gamestate["players"][i]["name"].ToString();
            Debug.Log("Player " + (i+1) + ": " + name);
            switch (i)
            {
                case 0:
                    name_0.text = name;
                    break;
                case 1:
                    name_1.text = name;
                    break;
                case 2:
                    name_2.text = name;
                    break;
                case 3:
                    name_3.text = name;
                    break;
                case 4:
                    name_4.text = name;
                    break;
                case 5:
                    name_5.text = name;
                    break;
                case 6:
                    name_6.text = name;
                    break;
                case 7:
                    name_7.text = name;
                    break;

            }
        }
    }

    // updates each player's chip count on screen
    // load_player_displays must be called first
    public static void update_player_chips(string json)
    {
        Debug.Log("Updating player chip counts");
        var gamestate = JsonMapper.ToObject(json);
        // get the player chip count and apply it to the correct text display
        for (int i = 0; i < DebugChangeCard.gameGlobals.numGamePlayers; ++i)
        {
            string chips = gamestate["players"][i]["chips"].ToString();
            switch (i)
            {
                case 0:
                    chips_0.text = chips;
                    break;
                case 1:
                    chips_1.text = chips;
                    break;
                case 2:
                    chips_2.text = chips;
                    break;
                case 3:
                    chips_3.text = chips;
                    break;
                case 4:
                    chips_4.text = chips;
                    break;
                case 5:
                    chips_5.text = chips;
                    break;
                case 6:
                    chips_6.text = chips;
                    break;
                case 7:
                    chips_7.text = chips;
                    break;
            }
        }
    }

    }