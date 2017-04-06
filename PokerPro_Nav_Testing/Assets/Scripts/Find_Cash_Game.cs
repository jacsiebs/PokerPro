using Facebook.Unity;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Find_Cash_Game : MonoBehaviour {

    // asks the server to join a game and saves the game id once a game is found
    // Note: the response will not occur until the game is starting
    IEnumerator Start () {
        // ask the server's matchmaker for a gameID 
        // the playerID is simply the facebook user token (unique)
        Debug.Log("Asking server for a game");
        string url = "http://104.131.99.193/join/" + GlobalVars.player_id;
		Debug.Log ("THIS IS THE URL WE ARE REQUESTING: " + url);
        WWW www = new WWW(url);
        // wait for request to complete
        yield return www;
        // check for errors
        if (www.error != null)
        {
            Debug.Log("WWW Error on Matchmaking Request:\n" + www.error);
        }
        else
        {
            // get the game id and switch to the game view
            string jsonString = www.text;
            var playerData = JsonMapper.ToObject(jsonString);
            GlobalVars.game_id = (string) playerData["gameId"];
            Debug.Log("Game found with id: " + GlobalVars.game_id);
            // switch to the game screen
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game_Scene");
        }
    }
	
	void Update () {
		
	}
}
