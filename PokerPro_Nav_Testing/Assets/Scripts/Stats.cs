using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour {

    private int num_stats = 7;
    
    // the Text objects which display the stats on screen
    // must be loaded by load_objects()
    private Text[] displays;
    private string jsonString;// holds the player object before parsing

    /* Stats displayed on screen and their indicies:
     *   Hands Played - 0 
     *   Hands Won - 1
     *   Hands Lost - 2
     *   Chips Won - 3
     *   Chips Lost - 4
     *   Net Chip Gain - 5
     *   ELO - 6
     */

	// Use this for initialization
	void Awake () {
        load_rank();// load the rank name and sprite - already located in globalVars
        load_objects();// load the gameobject text displays
        print_stats(load_statistics());// ask the server for stats and print to screen
	}

    private void load_rank()
    {
        Image rank_image = GameObject.Find("Rank_Image").GetComponent<Image>();
        // main menu must have already set the globalvars.rank sprite
        rank_image.sprite = GlobalVars.rank_sprite;
        // rank text unimplemented
//        rank_text.text = GlobalVars.rank;
    }

    private void print_stats(string[] stats)
    {
        for(int i = 0; i < num_stats; ++i)
        {
            displays[i].text = stats[i];
        }
    }
	
    // load gameobjects which hold each statistic
    private void load_objects()
    {
        displays = new Text[num_stats];

        for(int i = 0; i < num_stats; ++i)
        {
            displays[i] = GameObject.Find("Stat_" + i + "_txt").GetComponent<Text>();
            if(displays[i] == null)
            {
                Debug.Log("Can't find the text display for stat " + i);
            }
        }
    }

    // returns an array of the statistic values (index coresponds to the object index the stat is displayed on)
    private string[] load_statistics()
    {
        string[] results = new string[num_stats];

        StartCoroutine(getPlayerObject());// call to server for player data
        Debug.Log(jsonString);
        var playerJson = JsonMapper.ToObject(jsonString);// parse
        string ELO = playerJson["elo"].ToString();
        int chipsWon = (int) playerJson["chipsWon"];
        int chipsLost = (int) playerJson["chipsLost"];
        int handsWon = (int) playerJson["handsWon"];
        int handsLost = (int) playerJson["handsLost"];

        // Hands Played - 0
        results[0] = (handsWon + handsLost).ToString();
        // Hands Won - 1
        results[1] = handsWon.ToString();
        // Hands Lost - 2
        results[2] = handsLost.ToString();
        // Chips Won - 3
        results[3] = chipsWon.ToString();
        // Chips Lost - 4
        results[4] = chipsLost.ToString();
        // Net Chip Gain -5
        results[5] = (chipsWon - chipsLost).ToString();
        // ELO - 6
        results[6] = ELO.ToString();

        return results;
    }

    private IEnumerator getPlayerObject()
    {
        Debug.Log("Asking server for player info...");
        string url = "http://104.131.99.193/playerStats/" + GlobalVars.player_id;
        Debug.Log(url);
        WWW www = new WWW(url);
        // wait for a response
        WaitForSeconds w;
        while (!www.isDone)
            w = new WaitForSeconds(0.1f);
        Debug.Log("Got a player object for statistics.");
        jsonString = www.text;
        yield return www;
    }
	
}
