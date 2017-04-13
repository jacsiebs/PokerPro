using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;

public class DebugChangeCard : MonoBehaviour
{
    private Text[] playerIDs;
    private CardModel[] cardModel = new CardModel[25];
    private int[] myCards = new int[2];
    private int[] commonCards = new int[5];
    private string jsonString;
    public static JsonData gameState;
    private int numGamePlayers;
    //we want to keep track of how many cards are in play so we know how many to recall
    private int cardsInPlay = 0;

    public static class gameGlobals
    {
        // this value is the player's number for seating at the table
        public static int me;
        // this value is for the total number of players in a game
        public static int numGamePlayers;
        // this indicates if the game state has been loaded yet
        public static bool isLoaded;
        public static int numCCards;
        //bools for dealing cards
        // need to add logic to reset these when a new hand begins
        public static bool cc1;
        public static bool cc2;
        public static bool cc3;
        // hand count holder
        public static string handNum;
        // recall and deal vars
        public static bool recallVar;
    }

    void Start()
    {      
        // this will be uncommented once matchmaking is successful.
        // this method has been manually tested and works well/
        // manual test included creating a game via web browser and hard coding the game_id
        // and player_id.
        StartCoroutine(JSONGameState(GlobalVars.game_id, GlobalVars.player_id));
    }

    void Update()
    {
        StartCoroutine(checkingForCCards());
        //StartCoroutine(checkNewState());
    }

    void Awake()
    {
        gameGlobals.isLoaded = false;
        List<GameObject> cards = new List<GameObject>();
        //find all the card objects in the scene and add them to a list to reference later
        for (int k = 25; k > -1; k--)
        {
            cards.Add(GameObject.Find("Card" + k.ToString()));
        }
        for (int k = 0; k < 25; k++)
        {
            cardModel[k] = cards[k].GetComponent<CardModel>();
        }
    }

    //this method gets the JSON from the server and stores it as a object.
    // it also sets a few useful global variables.
    private IEnumerator JSONGameState(string gameID, string playerID)
    {
        string url = "http://104.131.99.193/game/" + gameID + '/' + playerID;
        Debug.Log("Asking for a new game state");
        WWW www = new WWW(url);
        yield return www;
        Debug.Log("Got a new game state.");
        jsonString = www.text;
		Debug.Log (jsonString); //REMOVE latger
        var gameStateJson = JsonMapper.ToObject(jsonString);
        gameState = gameStateJson;
        numGamePlayers = gameState["players"].Count;
        gameGlobals.numGamePlayers = numGamePlayers; //clean this up later
        gameGlobals.me = (int)gameState["me"];
        gameGlobals.handNum = gameState["hand"].ToString();
        gameGlobals.isLoaded = true;
        Debug.Log("Pot: " + gameState["pot"]);// delete this
        GlobalVars.Pot = (int) gameState["pot"];
		Debug.Log("Whose turn is it?");
		dealCards (); //deal cards to players
		if (isTurn())
		{
			Debug.Log("It's my turn");
			//enable bet buttons
			Disable_Buttons.enableButtons();
			//make a bet
		}
		else
		{
			Debug.Log("Not my turn");
			//make sure bottons are bisabled, do nothing
			Disable_Buttons.disableButtons();
			StartCoroutine (getUpdatedGameState ());
		}
    }

    //this method is used to long poll the server for an updated game state,
    public static IEnumerator getUpdatedGameState()
    {
        Debug.Log("Submitting gamestate request.");
        string url = "http://104.131.99.193/game/" + GlobalVars.game_id + '/' + GlobalVars.player_id;
        WWW www = new WWW(url);
        yield return www;
        Debug.Log("Got a gamestate.");
        string jsonString = www.text;
        Debug.Log(jsonString); //REMOVE later
        var gameStateJson = JsonMapper.ToObject(jsonString);
        gameState = gameStateJson;
        gameGlobals.numGamePlayers = gameState["players"].Count;
        gameGlobals.me = (int)gameState["me"];
        gameGlobals.numCCards = gameState["commonCards"].Count;
        gameGlobals.isLoaded = true;
        Debug.Log("Pot: " + gameState["pot"]);// delete this
        GlobalVars.Pot = (int)gameState["pot"];
        checkNewRound(gameState["hand"].ToString());
        // do we need to reset game state vales here? Might not need to   
        Debug.Log("Whose turn is it?");
        if (isTurn())
        {
            Debug.Log("It's my turn");
            //enable bet buttons and bet slider
            UpdateBet.enableSlider();
            Disable_Buttons.enableButtons();
            // finished, getUpdatedGamestate will becalled again by submit bet
        }
        else
        {
            Debug.Log("Not my turn");
            //make sure bottons are bisabled, do nothing
            Disable_Buttons.disableButtons();
            getUpdatedGameState();
        }
    }

    // check to see if we have a new hand dealt
    // this indicated that a new hand is happening and that we should 
    // clear vars and restart the initial state grab.
    // this will need to happen each time we request a new state
    public static void checkNewRound(string currentRound)
    {
        if (!gameGlobals.handNum.Equals(currentRound))
        {
            Debug.Log("We have a new hand!");
            // reset some globals
            gameGlobals.cc1 = false;
            gameGlobals.cc2 = false;
            gameGlobals.cc3 = false;
            gameGlobals.recallVar = true;
            // reset current hand
            gameGlobals.handNum = gameState["hand"].ToString();

            if (isTurn())
            {
                UpdateBet.enableSlider();
                Disable_Buttons.enableButtons();
                // wait to bet
            }
            else
            {
                Debug.Log("STARTING NEW LOOP");
                getUpdatedGameState();
            }
        }
        else
        {
            // not new hand, do nothing
        }
    }

    //this method is used to check if it is the user's turn or not.
    //this method should be called everytime we get a new game state.
    private static bool isTurn()
    {
        string currentPlayer = (string) gameState["currentPlayer"];
        if (currentPlayer.Equals(GlobalVars.player_id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //this method reads in the server card assignments and converts them to an int from 0-51
    // to correspond with the correct card face.
    private void parseMyCards()
    {
        //using convertToInt(string rank, string type) defined at the bottom of this script
        myCards[0] = convertToInt(gameState["players"][gameGlobals.me]["cards"][0]["rank"].ToString(), gameState["players"][gameGlobals.me]["cards"][0]["type"].ToString());
        myCards[1] = convertToInt(gameState["players"][gameGlobals.me]["cards"][1]["rank"].ToString(), gameState["players"][gameGlobals.me]["cards"][1]["type"].ToString());
        //for debugging purposes print:
        print("FIRST CARD: " + myCards[0]);
        print("SECOND CARD: " + myCards[1]);
    }

    // similar to parseMyCards() this parses the cards dealt to the river
    private void parseCommonCards()
    {
        //converts the common cards in json to ints for displaying
        //we only get 3 common cards to start
        for (int c = 0; c < 3; c++)
        {
            commonCards[c] = convertToInt(gameState["commonCards"][c]["rank"].ToString(), gameState["commonCards"][c]["type"].ToString());
        }
        //for debugging purposes print:
        print("FIRST CARD: " + commonCards[0]);
        print("SECOND CARD: " + commonCards[1]);
        print("THIRD CARD: " + commonCards[2]);
    }

    private void parseCommonCardsPlusOne()
    {
        //4th common card dealt
        commonCards[3] = convertToInt(gameState["commonCards"][3]["rank"].ToString(), gameState["commonCards"][3]["type"].ToString());
        //for debugging purposes print:
        print("FOURTH CARD: " + commonCards[3]);
    }

    private void parseCommonCardsPlusTwo()
    {
        //5th common card dealt
        commonCards[4] = convertToInt(gameState["commonCards"][4]["rank"].ToString(), gameState["commonCards"][4]["type"].ToString());
        //for debugging purposes print:
        print("FIFTH CARD: " + commonCards[4]);
    }

    private IEnumerator checkingForCCards()
    {
        //Debug.Log ("BEFORE");
        yield return null;
        if (gameGlobals.recallVar)
        {
            // recall cards to deck
            recallCards();
            dealCards(); // re-deal cards right after we recall them
            gameGlobals.recallVar = false; // to make sure this happens once
        }
        //Debug.Log ("Num Common Cards:" + gameGlobals.numCCards);
        if (gameGlobals.numCCards == 3 && !gameGlobals.cc1)
        {
            gameGlobals.cc1 = true;
            dealRiverF3(); //we might need to make this a coroutine
        }
        else if (gameGlobals.numCCards == 4 && !gameGlobals.cc2)
        {
            gameGlobals.cc2 = true;
            dealRiverP1();
        }
        else if (gameGlobals.numCCards == 5 && !gameGlobals.cc3)
        {
            gameGlobals.cc3 = true;
            dealRiverP2();
        }
        else
        {
            //do nothing and wait for a state change
        }
    }

    /*private static void handleNewHand()
    {
        //Debug.Log("Hand: " + gameState["hand"].ToString());
        // StopAllCoroutines();
        // reset some globals
        gameGlobals.cc1 = false;
        gameGlobals.cc2 = false;
        gameGlobals.cc3 = false;
        // reset current hand
        gameGlobals.handNum = gameState["hand"].ToString();

        if (isTurn())
        {
            UpdateBet.enableSlider();
            Disable_Buttons.enableButtons();
            // wait to bet
        }
        else
        {
            Debug.Log("STARTING NEW LOOP");
            getUpdatedGameState();
        }
    }*/

    private void dealCards()
    {
        //X players
        int numGamePlayers = gameGlobals.numGamePlayers;
        //set card travel speed
        float cardSpeed = 0.35f;
        for (int k = 0; k < numGamePlayers; k++)
        {
            cardModel[k].StartFly(k, cardSpeed);
            cardsInPlay++;
        }
        for (int i = 0; i < numGamePlayers; i++)
        {
            cardModel[i + numGamePlayers].StartFly(i + 8, cardSpeed);
            cardsInPlay++;
        }
        //parse the cards from the server to the app index of 0-51:
        parseMyCards();
        //Flip player's card for them to see
        int playerSeatPlaceholder = gameGlobals.me;
        int cardIndex0 = myCards[0];
        int cardIndex1 = myCards[1];

        //flip the two player cards for them to see
        cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder].cardBackOrig,
            cardModel[playerSeatPlaceholder].cardFaces[cardIndex0], playerSeatPlaceholder, numGamePlayers, cardSpeed);

        cardModel[playerSeatPlaceholder + numGamePlayers].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder + numGamePlayers].cardBackOrig,
            cardModel[playerSeatPlaceholder + numGamePlayers].cardFaces[cardIndex1], playerSeatPlaceholder + numGamePlayers, numGamePlayers, cardSpeed);
    }

    private void dealRiverF3()
    {
        parseCommonCards();
        int numGamePlayers = gameGlobals.numGamePlayers;
        for (int i = 0; i < 3; i++)
        {
            float cardSpeed = 0.35f;
            cardModel[2 * numGamePlayers + i].StartRiverDeal(i, cardSpeed);
            cardsInPlay++;
            //use cards dealt from server, commented becuase we can't proceed past matchmaking
            int cardIndex = commonCards[i];
            //we want to show right away, so set this to 0 for no delay
            cardModel[2 * numGamePlayers + i].GetComponent<CardFlipper>().FlipCard(cardModel[2 * numGamePlayers + i].cardBackOrig,
                cardModel[2 * numGamePlayers + i].cardFaces[cardIndex], (2 * numGamePlayers + i), 0, cardSpeed);
        }
    }

    private void dealRiverP1()
    {
        parseCommonCardsPlusOne();
        int numGamePlayers = gameGlobals.numGamePlayers;
        for (int i = 3; i < 4; i++)
        {
            float cardSpeed = 0.35f;
            cardModel[2 * numGamePlayers + i].StartRiverDeal(i, cardSpeed);
            cardsInPlay++;
            //use cards dealt from server
            int cardIndex = commonCards[i];
            //we want to show right away, so set this to 0 for no delay
            cardModel[2 * numGamePlayers + i].GetComponent<CardFlipper>().FlipCard(cardModel[2 * numGamePlayers + i].cardBackOrig,
                cardModel[2 * numGamePlayers + i].cardFaces[cardIndex], (2 * numGamePlayers + i), 0, cardSpeed);
        }
    }

    private void dealRiverP2()
    {
        parseCommonCardsPlusTwo();
        int numGamePlayers = gameGlobals.numGamePlayers;
        for (int i = 4; i < 5; i++)
        {
            float cardSpeed = 0.35f;
            cardModel[2 * numGamePlayers + i].StartRiverDeal(i, cardSpeed);
            cardsInPlay++;
            //use cards dealt from server
            int cardIndex = commonCards[i];
            //we want to show right away, so set this to 0 for no delay
            cardModel[2 * numGamePlayers + i].GetComponent<CardFlipper>().FlipCard(cardModel[2 * numGamePlayers + i].cardBackOrig,
                cardModel[2 * numGamePlayers + i].cardFaces[cardIndex], (2 * numGamePlayers + i), 0, cardSpeed);
        }
    }

    private void recallCards()
    {
        float cardSpeed = 0.35f;
        for (int i = 0; i < cardsInPlay; i++)
        {
            cardModel[i].StartRecall(cardSpeed);
        }
        cardsInPlay = 0;
    }

    // These are the graphical tests, which include some game state testing as well
    // The 'player' is always Player 2
    // duplicates are only checked for local hands, so some may occur within other hands
    // or the river. This is okay, as it is just to demo how the machanics work. The server
    // will provide non-duplicates when working correctly.
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 175, 28), "Deal from deck: 2 Players"))
        {
            //simulate 2 players
			int numGamePlayers = 2;
            //set card travel speed
            float cardSpeed = 0.35f;
			for (int k = 0; k < numGamePlayers; k++) 
			{
				cardModel[k].StartFly(k, cardSpeed);
				cardsInPlay++;
			}
            for (int i = 0; i < numGamePlayers; i++)
            {
				cardModel[i+numGamePlayers].StartFly(i+8, cardSpeed);
                cardsInPlay++;
            }
            // call the following method when the game state is active, need matchmaking to work first:
            parseMyCards();
            //Flip player's card for them to see
            int playerSeatPlaceholder = 1;
            //the following two lines are for when the matchmaking succeeds and the game state is pulled:
            int cardIndex0 = myCards[0];
            int cardIndex1 = myCards[1];
            // if we want to see what the cards are, we can uncomment the following two lines:
            //print(cardIndex0);
            //print(cardIndex1);
            //flip the two player cards for them to see
            cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder].cardBackOrig, 
                cardModel[playerSeatPlaceholder].cardFaces[cardIndex0], playerSeatPlaceholder, numGamePlayers, cardSpeed);

            cardModel[playerSeatPlaceholder+1].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder+1].cardBackOrig, 
                cardModel[playerSeatPlaceholder+1].cardFaces[cardIndex1], playerSeatPlaceholder+1, numGamePlayers, cardSpeed);
        }

        // this test recalls all the cards currently in play
        // you can think of this as a psudo-shuffle animation
        if(GUI.Button(new Rect(10, 50, 120, 28), "Recall cards"))
        {
            float cardSpeed = 0.35f;
            for (int i = 0; i < cardsInPlay; i++)
            {
                cardModel[i].StartRecall(cardSpeed);
            }
            cardsInPlay = 0;
        }

        if (GUI.Button(new Rect(10, 90, 180, 28), "Simulate fold for Player 7"))
        {
            //Player 6 will have cardModels[6] and cardModels[14]
            //this could be derrived from a game state check instead of hard coded
            cardModel[6].PlayerFold();
            cardModel[14].PlayerFold();
        }

        //we want to provide the option for players to show their hands at the end of the round.
        //we need to add a button for this for the next iteration, but this simulates what it does.
        if (GUI.Button(new Rect(10, 130, 210, 28), "Simulate show for Players 1, 4, 7"))
        {
            for (int i = 0; i < 8; i+=3)
            {
                float cardSpeed = 0.35f;
                //Flip player's card for them to see
                int playerSeatPlaceholder = i;
                //Simulate deal from shuffle deck from server
                int cardIndex0 = UnityEngine.Random.Range(0, 51);
                int cardIndex1 = cardIndex0;
                //check for no duplicates
                while (cardIndex1 == cardIndex0)
                {
                    cardIndex1 = UnityEngine.Random.Range(0, 51);
                }
                //we want to show right away, so set this to 0 for no delay
                int numGamePlayers = 0;
                //flip the cards
                cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder].cardBackOrig,
                    cardModel[playerSeatPlaceholder].cardFaces[cardIndex0], playerSeatPlaceholder, numGamePlayers, cardSpeed);

                cardModel[playerSeatPlaceholder + 8].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder + 8].cardBackOrig,
                    cardModel[playerSeatPlaceholder + 8].cardFaces[cardIndex1], playerSeatPlaceholder + 8, numGamePlayers, cardSpeed);
            }
        }

        if (GUI.Button(new Rect(10, 170, 180, 28), "Simulate river dealing"))
        {
            //simulate for numGamePlayers = 8
            int numGamePlayers = 8;
            for (int i = 0; i < 3; i++)
            {
                float cardSpeed = 0.35f;
                cardModel[2*numGamePlayers+i].StartRiverDeal(i, cardSpeed);
                cardsInPlay++;
                //use cards dealt from server, commented becuase we can't proceed past matchmaking
                // uncomment to activte. Tested manually and it works.
                //int cardIndex = commonCards[i];
                // for now we will simulate a card from there server with:
                int cardIndex = UnityEngine.Random.Range(0, 51);
                //we want to show right away, so set this to 0 for no delay
                cardModel[2*numGamePlayers + i].GetComponent<CardFlipper>().FlipCard(cardModel[2*numGamePlayers + i].cardBackOrig,
                    cardModel[2*numGamePlayers + i].cardFaces[cardIndex], (2*numGamePlayers + i), 0, cardSpeed);
            }
        }

        if (GUI.Button(new Rect(200, 10, 200, 28), "Simulate river dealing + 1st card"))
        {
            //simulate for numGamePlayers = 8
            int numGamePlayers = 8;
            for (int i = 3; i < 4; i++)
            {
                float cardSpeed = 0.35f;
                cardModel[2 * numGamePlayers + i].StartRiverDeal(i, cardSpeed);
                cardsInPlay++;
                //use cards dealt from server, commented for reasons outlined above
                //int cardIndex = commonCards[i];
                //simulate server dealt card:
                int cardIndex = UnityEngine.Random.Range(0, 51);
                //we want to show right away, so set this to 0 for no delay
                cardModel[2 * numGamePlayers + i].GetComponent<CardFlipper>().FlipCard(cardModel[2 * numGamePlayers + i].cardBackOrig,
                    cardModel[2 * numGamePlayers + i].cardFaces[cardIndex], (2 * numGamePlayers + i), 0, cardSpeed);
            }
        }

        if (GUI.Button(new Rect(410, 10, 200, 28), "Simulate river dealing + 2nd card"))
        {
            //simulate for numGamePlayers = 8
            int numGamePlayers = 8;
            for (int i = 4; i < 5; i++)
            {
                float cardSpeed = 0.35f;
                cardModel[2 * numGamePlayers + i].StartRiverDeal(i, cardSpeed);
                cardsInPlay++;
                //use cards dealt from server
                //int cardIndex = commonCards[i];
                //simulate server dealt card:
                int cardIndex = UnityEngine.Random.Range(0, 51);
                //we want to show right away, so set this to 0 for no delay
                cardModel[2 * numGamePlayers + i].GetComponent<CardFlipper>().FlipCard(cardModel[2 * numGamePlayers + i].cardBackOrig,
                    cardModel[2 * numGamePlayers + i].cardFaces[cardIndex], (2 * numGamePlayers + i), 0, cardSpeed);
            }
        }
    }

    //this converts the server given data 'rank' and 'type' to a card face index from 0-51
    private int convertToInt(string rank, string type)
    {
        //ace
        if (rank == "A")
        {
            if (type == "C")
            {
                return 38;
            }
            if (type == "D")
            {
                return 25;
            }
            if (type == "H")
            {
                return 12;
            }
            if (type == "S")
            {
                return 51;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "2")
        {
            if (type == "C")
            {
                return 26;
            }
            if (type == "D")
            {
                return 13;
            }
            if (type == "H")
            {
                return 0;
            }
            if (type == "S")
            {
                return 39;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "3")
        {
            if (type == "C")
            {
                return 27;
            }
            if (type == "D")
            {
                return 14;
            }
            if (type == "H")
            {
                return 1;
            }
            if (type == "S")
            {
                return 40;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "4")
        {
            if (type == "C")
            {
                return 28;
            }
            if (type == "D")
            {
                return 15;
            }
            if (type == "H")
            {
                return 2;
            }
            if (type == "S")
            {
                return 41;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "5")
        {
            if (type == "C")
            {
                return 29;
            }
            if (type == "D")
            {
                return 16;
            }
            if (type == "H")
            {
                return 3;
            }
            if (type == "S")
            {
                return 42;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "6")
        {
            if (type == "C")
            {
                return 30;
            }
            if (type == "D")
            {
                return 17;
            }
            if (type == "H")
            {
                return 4;
            }
            if (type == "S")
            {
                return 43;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "7")
        {
            if (type == "C")
            {
                return 31;
            }
            if (type == "D")
            {
                return 18;
            }
            if (type == "H")
            {
                return 5;
            }
            if (type == "S")
            {
                return 44;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "8")
        {
            if (type == "C")
            {
                return 32;
            }
            if (type == "D")
            {
                return 19;
            }
            if (type == "H")
            {
                return 6;
            }
            if (type == "S")
            {
                return 45;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "9")
        {
            if (type == "C")
            {
                return 33;
            }
            if (type == "D")
            {
                return 20;
            }
            if (type == "H")
            {
                return 7;
            }
            if (type == "S")
            {
                return 46;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "10")
        {
            if (type == "C")
            {
                return 34;
            }
            if (type == "D")
            {
                return 21;
            }
            if (type == "H")
            {
                return 8;
            }
            if (type == "S")
            {
                return 47;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "J")
        {
            if (type == "C")
            {
                return 35;
            }
            if (type == "D")
            {
                return 22;
            }
            if (type == "H")
            {
                return 9;
            }
            if (type == "S")
            {
                return 48;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "Q")
        {
            if (type == "C")
            {
                return 36;
            }
            if (type == "D")
            {
                return 23;
            }
            if (type == "H")
            {
                return 10;
            }
            if (type == "S")
            {
                return 49;
            }
            else
            {
                return 0;
            }
        }
        if (rank == "K")
        {
            if (type == "C")
            {
                return 37;
            }
            if (type == "D")
            {
                return 24;
            }
            if (type == "H")
            {
                return 11;
            }
            if (type == "S")
            {
                return 50;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

}
