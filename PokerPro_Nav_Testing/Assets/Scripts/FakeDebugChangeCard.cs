using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDebugChangeCard : MonoBehaviour {

    static FakeGlobalVars globals;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static string JSONGameState(string gameID, string playerID, string request)
    {
        string url = "http://104.131.99.193/game/" + gameID + '/' + playerID;
        Debug.Log("Asking for a new game state");
        //WWW www = new WWW(url);
        // gameState already set with prefabricated JSON
        //yield return www;
        Debug.Log("Got a new game state.");
        //jsonString = www.text;
        var gameStateJson = globals.gameState;
        int numGamePlayers = gameState["players"].Count;
        gameGlobals.numGamePlayers = numGamePlayers;
        gameGlobals.me = (int)gameState["me"];
        string Pot = (string) gameState["pot"];
        Debug.Log("Whose turn is it?");
        if (request.Equals("players"))
        {
            return numGamePlayers.ToString();
        }
        else if (request.Equals("pot"))
        {
            return Pot;
        }

    }
}
