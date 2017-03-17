using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Find_Cash_Game : MonoBehaviour {
    
    void Start () {
       
    }
	
	void Update () {
		
	}

    // asks the server to join a game and saves the game id once a game is found
    IEnumerator getGameId() {
        // ask the server's matchmaker for a gameID 
        // the playerID is simply the facebook user token (unique)
        Debug.Log("Asking server for a game.");
        string url = "http://104.131.99.193/join/" + GlobalVars.player_id;
        UnityWebRequest www = UnityWebRequest.Get(url);
        // wait for request to complete
        yield return www.Send();
        // check for errors
        if (www.isError)
        {
            Debug.Log("WWW Error on Matchmaking Request: " + www.error);
        }
        else
        {
            GlobalVars.game_id = www.downloadHandler.text;
            print("Game found with id: " + GlobalVars.game_id);
            // switch to the game screen
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game_Scene");
        }

    }

    // used for on click
    public void getCashGame() {
        // switch to loading screen
        UnityEngine.SceneManagement.SceneManager.LoadScene("Matchmaking_Scene");
        StartCoroutine(getGameId());
        
    }
}
