using System.Collections;
using UnityEngine;
using System;
using LitJson;

public class getGameState : MonoBehaviour {

    private string jsonString;
    public JsonData gameState;
    
	private void Start()
	{  
		StartCoroutine(JSONGameState (10, 200));
	}

	private IEnumerator JSONGameState(int gameID, int playerID)
	{
		string url = "http://104.131.99.193/game/" + gameID.ToString() + '/' + playerID.ToString();
		WWW www = new WWW(url);
		yield return www;
		jsonString = www.text;
        var gameStateJson = JsonMapper.ToObject(jsonString);
        gameState = gameStateJson;
    }   
}
