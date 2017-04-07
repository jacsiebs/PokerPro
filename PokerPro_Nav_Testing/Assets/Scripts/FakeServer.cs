using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class FakeServer 
{
    public string error = null;
    public string text = "";
    public string url = "";

    public void send(string url)
    {
        this.url = url;
    }
    /*	public static JsonData fakeGameState;

        // Use this for initialization
        void Start()
        {

        }

        void Awake()
        {
            //parse an fake game state with two players: 20 , 30
            string JSONinput = "{\"tournamentId\":\"1\",\"buyin\":1000000,\"game\":1,\"hand\":1,\"spinCount\":0,\"sb\":0,\"pot\":0,\"sidepots\":[],\"commonCards\":[],\"db\":0,\"callAmount\":0,\"minimumRaiseAmount\":0,\"players\":[{\"id\":\"20\",\"name\":\"xXP0k3RsLaY3rXx\",\"status\":\"active\",\"chips\":1000000,\"chipsBet\":0},{\"id\":\"30\",\"name\":\"xXP0k3RsLaY3rXx\",\"status\":\"active\",\"chips\":1000000,\"chipsBet\":0,\"cards\":[{\"rank\":\"2\",\"type\":\"C\"},{\"rank\":\"K\",\"type\":\"S\"}]}],\"me\":1,\"currentPlayer\":\"20\"}";
            fakeGameState = JsonMapper.ToObject (JSONinput);
        }

        public static JsonData makeBet(string betURL)
        {
            string[] input = betURL.Split ('/');
            //Sample input for reference: http://104.131.99.193/game/1/20/10000
            //so the values we are concered with are at 5 and 6
            for (int k = 0; k < fakeGameState ["players"].Count (); k++) 
            {
                if (fakeGameState ["players"] [k] ["id"].Equals (input [5])) 
                {
                    fakeGameState["players"][k]["chipsBet"] = input[6];
                    fakeGameState["players"][k]["chips"] = ((int)fakeGameState["players"][(int)fakeGameState["me"]]["chips"]
                        - (int)input[6]).ToString();
                    return fakeGameState;
                }
            }
        }*/
}