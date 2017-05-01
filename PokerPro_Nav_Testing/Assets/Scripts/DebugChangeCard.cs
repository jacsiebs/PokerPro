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
    private static string jsonString;
    public static JsonData gameState;
    private int numGamePlayers;
    private static WWW www = null;
    //we want to keep track of how many cards are in play so we know how many to recall
    private int cardsInPlay = 0;
    // slider obj
    private Slider theSlider;
    // dealter button
    private DealerButtonAssign dealerButton;

    public static class gameGlobals
    {
        // this value is the player's number for seating at the table
        public static int me;
        // this value is for the total number of players in a game
        public static int numGamePlayers;
        public static bool[] isActive;// is the ith player active?
        public static int[] av_nums;// the ith players avatar number
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
        // www finished 
        public static bool wwwLoaded;
        // deal cards?
        public static bool dealVar = false;
        // cards finsihed dealing animation for help flashing on inital game state
        public static bool dealtCards = false;
        // true only for first game state
        public static bool init = true;
        public static bool leaveReady = false;
    }

    public static class Stats {
        public static int old_chip_count = GlobalVars.init_chips;
        public static int handsWon = 0;
        public static int handsLost = 0;
        public static int handsPlayed = 0;
        public static int chipsWon = 0;
        public static int chipsLost = 0;
        public static int netChipGain = 0;
        public static int ELO = GlobalVars.ELO;
    }

    void Start()
    {
        // this will be uncommented once matchmaking is successful.
        // this method has been manually tested and works well/
        // manual test included creating a game via web browser and hard coding the game_id
        // and player_id.
        JSONGameState(GlobalVars.game_id, GlobalVars.player_id);
    }

    void Update()
    {
        //StartCoroutine(checkNewState());
        if(gameGlobals.leaveReady)
        {
            StartCoroutine(leaveHelper());
        }
        parseGameState();
        checkingForCCards();
        if (0 < GlobalVars.AFK && GlobalVars.AFK <= 2)
        {
            fold(); // this acts like a check/fold
        }
        if (GlobalVars.AFK == 3)
        {
            leaveGame(); // kick player who mises more than 2 turns
        }
    }

    void Awake()
    {
        gameGlobals.wwwLoaded = false;
        gameGlobals.isLoaded = false;
        List<GameObject> cards = new List<GameObject>();
        gameGlobals.handNum = "1";

        Debug.Log("Awaken: DebugChangeCards");

        //find all the card objects in the scene and add them to a list to reference later
        for (int k = 25; k > -1; k--)
        {
            cards.Add(GameObject.Find("Card" + k.ToString()));
        }
        for (int k = 0; k < 25; k++)
        {
            cardModel[k] = cards[k].GetComponent<CardModel>();
        }
        // find dealer chip
        GameObject temp2 = GameObject.Find("DealerChip");
        if (temp2 != null)
        {
            dealerButton = temp2.GetComponent<DealerButtonAssign>();
        }
        else
        {
            Debug.Log("Can't locate the dealer chip.");
        }
        // added from SubmitBet
        GameObject temp = GameObject.Find("Bet Slider");
        if (temp != null)
        {
            theSlider = temp.GetComponent<Slider>();
        }
        else
        {
            Debug.Log("Can't locate the slider.");
        }
        // end add
    }

    public bool[] get_active_players()
    {
        int i;
        gameGlobals.numGamePlayers = gameState["players"].Count;
        bool[] isActive = new bool[gameGlobals.numGamePlayers];
        // will be updated if players drop out
        for(i = 0;i < gameGlobals.numGamePlayers; ++i)
        {
            isActive[i] = true;
        }
        // disable all other player panels
        for(; i < 8; ++i)
        {
            GameObject.Find("Player_Panel_" + i).SetActive(false);
        }
        return isActive;
    }

    public int[] get_player_avatar_nums()
    {
        int[] av_nums = new int[gameGlobals.numGamePlayers];
        for(int i = 0; i < gameGlobals.numGamePlayers; ++i)
        {
            av_nums[i] = (int) gameState["players"][i]["avatarId"];
            // TODO this is where the code to load the facebook profile should be - unimplemented for players other than the user
            //      if a player has an facebook pic avatar -> set it to the default avatar
            if(av_nums[i] == 0)
            {
                av_nums[i] = 1;// default avatar
            }
        }
        return av_nums;
    }

    // use for on-click
    public void leaveGame()
    {
        updateStats();
        //updateELO(chip gain);
        sendStats(true);
        Debug.Log("Leaving the game...");
    }

    private IEnumerator leaveHelper()
    {
        string url = "http://104.131.99.193/leave/" + GlobalVars.game_id + "/" + GlobalVars.player_id;
        www = new WWW(url);
        yield return www;
        GlobalVars.game_id = "-1";
        // return to main menu
        Debug.Log("Game succesfully left. Returning to the main menu.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu_Scene");
    }

    // updates the server with the new stats
    private void sendStats(bool leavingGame)
    {
        Debug.Log("Sending updated stats to the server");
        // all stats should be up 
        // ELO should be updated from leaving the game
        StartCoroutine(sendAStat("elo", GlobalVars.ELO.ToString(), false, leavingGame));
        StartCoroutine(sendAStat("chipsWon", Stats.chipsWon.ToString(), false, leavingGame));
        StartCoroutine(sendAStat("chipsLost", Stats.chipsLost.ToString(), false, leavingGame));
        StartCoroutine(sendAStat("handsWon", Stats.handsWon.ToString(), false, leavingGame));
        StartCoroutine(sendAStat("handsLost", Stats.handsLost.ToString(), true, leavingGame));
    }

    private IEnumerator sendAStat(string statName, string statValue, bool lastCall, bool leavingGame)
    {
        string url = "http://104.131.99.193/changeStat/" + GlobalVars.player_id + "/" + statName + "/" + statValue;
        WWW post = new WWW(url);
        yield return post;
        Debug.Log("Sent " + statName + " to server.");
        Debug.Log(post.text);
        if(post.error != null)
        {
            Debug.Log("WWW error on sending stats: " + www.error);
        }
        // set leaveReady if this is the final stat update
        if(lastCall && leavingGame)
        {
            gameGlobals.leaveReady = true;
        }
    }

    //this method gets the JSON from the server and stores it as a object.
    // it also sets a few useful global variables.
    private void JSONGameState(string gameID, string playerID)
    {
        //askForGSMethod();
        StartCoroutine(askForInitialGameState());
    }

    public void parseGameState()
    {
        if (gameGlobals.wwwLoaded)
        {
            if (gameGlobals.init)
            {
                // only deal on init state
                gameGlobals.dealVar = true; // we flahs cards if turn here
                Debug.Log(jsonString); //REMOVE latger
                updateGameState();
                // remove player panels for players not in the game
                Avatar_Cropper.set_all_avatars(get_player_avatar_nums(), get_active_players());
                // set player names
                Debug.Log(jsonString);
                Display_Curr_Bet.update_player_names(jsonString);

                gameGlobals.init = false;

                if (isTurn())
                {
                    Debug.Log("It's my turn");
                    cardModel[gameGlobals.me].notifyTurn(myCards[0]);
                    cardModel[gameGlobals.me + gameGlobals.numGamePlayers].notifyTurn(myCards[1]);
                    //enable bet buttons
                    Disable_Buttons.enableButtons();
                    turnOnSlider();
                    //make a bet
                    gameGlobals.wwwLoaded = false;
                }
                else
                {
                    Debug.Log("Not my turn");
                    // flash opponents cards to indicate their turn
                    if (gameGlobals.dealtCards)
                    {
                        cardModel[(int)gameState["currentPlayer"]].notifyOpponentTurn();
                        cardModel[(int)gameState["currentPlayer"] + gameGlobals.numGamePlayers].notifyOpponentTurn();
                    }
                    //make sure bottons are bisabled, do nothing
                    Disable_Buttons.disableButtons();
                    turnOffSlider();
                    gameGlobals.wwwLoaded = false;
                    getUpdatedGameState();
                    //StartCoroutine(getUpdatedGameState());
                }

            }
            // non-init
            else 
            {
                Debug.Log(jsonString); //REMOVE latger
                updateGameState();
                // update chip counts on display
                Display_Curr_Bet.update_player_chips(jsonString);
                if (isTurn())
                {
                    Debug.Log("It's my turn");
                    cardModel[gameGlobals.me].notifyTurn(myCards[0]);
                    cardModel[gameGlobals.me + gameGlobals.numGamePlayers].notifyTurn(myCards[1]);
                    //enable bet buttons
                    Disable_Buttons.enableButtons();
                    turnOnSlider();
                    //make a bet
                    gameGlobals.wwwLoaded = false;
                }
                else
                {
                    Debug.Log("Not my turn\n" + jsonString);
                    // flash opponents cards to indicate their turn
                    if (gameGlobals.dealtCards)
                    {
                        cardModel[getOpponentPos()].notifyOpponentTurn();
                        cardModel[getOpponentPos() + gameGlobals.numGamePlayers].notifyOpponentTurn();
                    }
                    //make sure bottons are bisabled, do nothing
                    Disable_Buttons.disableButtons();
                    turnOffSlider();
                    gameGlobals.wwwLoaded = false;
                    getUpdatedGameState();
                    //StartCoroutine(getUpdatedGameState());
                }
            }
        }
        else
        {
            // do nothing
        }
    }

    public int getOpponentPos()
    {
        for (int i = 0; i < gameGlobals.numGamePlayers; i++)
        {
            if (gameState["players"][i]["id"].ToString().Equals(gameState["currentPlayer"].ToString()))
            {
                return i;
            }
        }
        return -1;
    }

    public void updateGameState()
    {
        var gameStateJson = JsonMapper.ToObject(jsonString);
        gameState = gameStateJson;
        numGamePlayers = gameState["players"].Count;
        gameGlobals.numGamePlayers = numGamePlayers; //clean this up later
        gameGlobals.me = (int)gameState["me"];
        gameGlobals.numCCards = gameState["commonCards"].Count;
        Debug.Log("Num Common Cards:" + gameGlobals.numCCards);
        //gameGlobals.handNum = gameState["hand"].ToString();
        gameGlobals.isLoaded = true;
        Debug.Log("Pot: " + gameState["pot"]);// delete this
        GlobalVars.Pot = (int)gameState["pot"];
        Debug.Log("Whose turn is it?");
        checkNewRound(gameState["hand"].ToString());
        // move dealer button to correct place
        dealerButton.StartFly(Int32.Parse(gameState["db"].ToString()));
    }

    public IEnumerator askForInitialGameState()
    {
        //yield return new WaitForSeconds(0.15f);
        string url = "http://104.131.99.193/fastgame/" + GlobalVars.game_id + '/' + GlobalVars.player_id;
        WWW www2 = new WWW(url);
        yield return www2;
        Debug.Log("Got a gamestate.");
        jsonString = www2.text;
        try
        {
            var testJson = JsonMapper.ToObject(jsonString);
            string playerCardsYet = (string) testJson["currentPlayer"];
        }
        catch (KeyNotFoundException e)
        {
            StartCoroutine(askForInitialGameState());
        }
        Debug.Log(jsonString);

        gameGlobals.wwwLoaded = true;
    }

    public IEnumerator askForGameState()
    {
        string url = "http://104.131.99.193/game/" + GlobalVars.game_id + '/' + GlobalVars.player_id;
        www = new WWW(url);
        yield return www;
        Debug.Log("Got a gamestate.");
        jsonString = www.text;
        gameGlobals.wwwLoaded = true;
    }

    //this method is used to long poll the server for an updated game state,
    //public IEnumerator getUpdatedGameState()
    public void getUpdatedGameState()
    {
        //yield return null;
        Debug.Log("Submitting gamestate request.");
        StartCoroutine(askForGameState());//askForGSMethod();
    }

    // check to see if we have a new hand dealt
    // this indicated that a new hand is happening and that we should 
    // clear vars and restart the initial state grab.
    // this will need to happen each time we request a new state
    public void checkNewRound(string currentRound)
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
            // update stats
            updateStats();
        }
        else
        {
            // not new hand, do nothing
        }
        //Debug.Log("Returning from checkNewRound.");
    }

    // updates the player stats for this hand
    public void updateStats()
    {
        // chips
        if (Stats.old_chip_count > GlobalVars.chips) {
            Stats.chipsLost = Stats.old_chip_count - GlobalVars.chips;
            Stats.netChipGain -= Stats.chipsLost;
            Stats.handsWon++;
        } else if(Stats.old_chip_count < GlobalVars.chips)
        {
            Stats.chipsWon = GlobalVars.chips - Stats.old_chip_count;
            Stats.netChipGain += Stats.chipsWon;
            Stats.handsWon--;
        }
        // hand count
        Stats.handsPlayed++;
        // update the old chip count with current chip count
        Stats.old_chip_count = GlobalVars.chips;
        // ELO not updated until the player leaves the game or the game ends
    }

    //this method is used to check if it is the user's turn or not.
    //this method should be called everytime we get a new game state.
    private static bool isTurn()
    {
        string currentPlayer = (string) gameState["currentPlayer"];
        if (currentPlayer.Equals(GlobalVars.player_id))
        {
            GlobalVars.isTurn = true;
            return true;
        }
        else
        {
            //Debug.Log("NOT MY TURN - from isTurn()");
            GlobalVars.isTurn = false;
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

    private void checkingForCCards()
    {
        if (gameGlobals.dealVar)
        {
            dealCards();
            gameGlobals.dealVar = false;
        }

        //Debug.Log ("BEFORE");
        //yield return null;
        if (gameGlobals.recallVar)
        {
            // recall cards to deck
            recallCards();
            dealCards(); // re-deal cards right after we recall them
            gameGlobals.recallVar = false; // to make sure this happens once
        }
        
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

    private void dealCards()
    {
        //X players
        int numGamePlayers = gameGlobals.numGamePlayers;
        //set card travel speed
        float cardSpeed = 0.35f;
        for (int k = 0; k < numGamePlayers; k++)
        {
            cardModel[k].StartFly(k, cardSpeed, numGamePlayers);
            cardsInPlay++;
        }
        // here we need to use 8 as an offset timer for 2nd pass of cards
        for (int i = 0; i < numGamePlayers; i++)
        {
            cardModel[i + numGamePlayers].StartFly(i + numGamePlayers, cardSpeed, numGamePlayers);
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

        //notify player if its their turn when they are dealt cards
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
    //*******
    // This starts the merging of SubmitBet, all methods below are from the old file
    // use this function in on click
    //*******
    public void sendTheBet()
    {
        // sends the current bet on the slider
        StartCoroutine(betHelper(false));
    }

    // sumbit a bet equal to the bet currently before you
    public void call()
    {
        // check if the player has enough chips
        // TODO replace with an all in
        if (GlobalVars.curr_bet > GlobalVars.chips)
        {
            Debug.Log("Not enough chips to call.");
            Display_Message.print_message("Not enough chips to call");
        }
        // valid call
        else
        {
            turnOffSlider();
            GlobalVars.bet = GlobalVars.curr_bet;
            StartCoroutine(betHelper(false));// run bet helper like normal
        }
    }

    // turns off the slider so that the bet can no longer be modified
    private void turnOffSlider()
    {
        UpdateBet.disableSlider();
    }

    // turns on the slider so that the bet can be modified
    private void turnOnSlider()
    {
        UpdateBet.enableSlider();
    }

    // submit a bet of all your chips
    // TODO handle the case where all in does not cover the call, currently it does not allow this
    public void allIn()
    {
        turnOffSlider();
        GlobalVars.bet = GlobalVars.chips;
        StartCoroutine(betHelper(false));// run bet helper like normal
    }

    // submit a fold
    public void fold()
    {
        turnOffSlider();
        GlobalVars.bet = 0;// set the bet to 0 
        StartCoroutine(betHelper(true));// run bet helper like normal
    }

    private IEnumerator betHelper(bool isFolded)
    {
        // check that the bet is valid - must be at least as large as the bet needed to call
        if (GlobalVars.bet < GlobalVars.curr_bet && !isFolded)
        {
            Debug.Log("Invalid bet. Your bet: " + GlobalVars.bet + "\nBet Needed: " + GlobalVars.curr_bet);
            Display_Message.print_message("Submit a bet over " + GlobalVars.curr_bet + " or fold.");
            turnOnSlider();// renable if disabled by all in
        }

        // bet is valid or a fold - submit the bet and get the gamestate in return
        else
        {
            if (isFolded)
                Debug.Log("Sending Fold...");
            else
                Debug.Log("Sending bet...");

            turnOffSlider();// disbale the bet slider

            // send the bet to the server
            string url = "http://104.131.99.193/game/" + GlobalVars.game_id + "/" + GlobalVars.player_id + "/" + GlobalVars.bet;
            WWW www = new WWW(url);
            yield return www;

            // error check
            if (www.error != null)
            {
                Debug.Log("WWW submit bet error: " + www.error);
            }
            else
            {
                Debug.Log("Bet of " + GlobalVars.bet + " sent.");
                if (GlobalVars.bet == 0 && !isFolded)
                    Display_Message.print_message("You have checked.");
                else if (isFolded)
                    Display_Message.print_message("You have folded.");
                else
                    Display_Message.print_message("You have bet " + GlobalVars.bet + ".");

                // subtract the bet from the chips
                GlobalVars.chips -= GlobalVars.bet;

                // update the gamestate
                Debug.Log("Sent bet and got a gamestate.");
                jsonString = www.text;
                Debug.Log(jsonString); // REMOVE later
                updateGameState();
                // reset the bet needed to call
                GlobalVars.curr_bet = 0;
                theSlider.value = 0;// also makes GlobalVars.bet=0

                if (isTurn())
                {
                    Debug.Log("It's my turn");
                    //enable bet buttons
                    Disable_Buttons.enableButtons();
                    turnOnSlider();
                    //make a bet
                    gameGlobals.wwwLoaded = false;
                }
                else
                {
                    Debug.Log("Not my turn");
                    //make sure bottons are bisabled, do nothing
                    Disable_Buttons.disableButtons();
                    turnOffSlider();
                    gameGlobals.wwwLoaded = false;
                    //StartCoroutine(getUpdatedGameState());
                    getUpdatedGameState();
                }
            }
        }
    }

    // These are the graphical tests, which include some game state testing as well
    // The 'player' is always Player 2
    // duplicates are only checked for local hands, so some may occur within other hands
    // or the river. This is okay, as it is just to demo how the machanics work. The server
    // will provide non-duplicates when working correctly.
    //----------------------------------------------------------------------------
    // *****UNCOMMENT BELOW TO ENABLE TESTING BUTTONS IF YOU WANT TO USE THEM*****
    //----------------------------------------------------------------------------
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 175, 28), "Deal from deck: 2 Players"))
        {
            //simulate 2 players
            int numGamePlayers = 8;
            //set card travel speed
            float cardSpeed = 0.35f;
            for (int k = 0; k < numGamePlayers; k++)
            {
                cardModel[k].StartFly(k, cardSpeed, numGamePlayers);
                cardsInPlay++;
            }
            for (int i = 0; i < numGamePlayers; i++)
            {
                cardModel[i + numGamePlayers].StartFly(i + numGamePlayers, cardSpeed, numGamePlayers);
                cardsInPlay++;
            }
            // call the following method when the game state is active, need matchmaking to work first:
            //parseMyCards();
            //Flip player's card for them to see
            int playerSeatPlaceholder = 1;
            //the following two lines are for when the matchmaking succeeds and the game state is pulled:
            int cardIndex0 = 2;//myCards[0];
            int cardIndex1 = 19;//myCards[1];
                                // if we want to see what the cards are, we can uncomment the following two lines:
                                //print(cardIndex0);
                                //print(cardIndex1);
                                //flip the two player cards for them to see

            cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder].cardBackOrig,
                cardModel[playerSeatPlaceholder].cardFaces[cardIndex0], playerSeatPlaceholder, numGamePlayers, cardSpeed);

            cardModel[playerSeatPlaceholder + numGamePlayers].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder + numGamePlayers].cardBackOrig,
                cardModel[playerSeatPlaceholder + numGamePlayers].cardFaces[cardIndex1], playerSeatPlaceholder + numGamePlayers, numGamePlayers, cardSpeed);

            dealerButton.StartFly(2);
        }

        // this test recalls all the cards currently in play
        // you can think of this as a psudo-shuffle animation
        if (GUI.Button(new Rect(10, 50, 120, 28), "Flash P3"))
        {
            cardModel[2].notifyOpponentTurn();
            cardModel[10].notifyOpponentTurn();
        }
    }
            /*private void OnGUI()
            {
                if (GUI.Button(new Rect(10, 10, 175, 28), "Deal from deck: 2 Players"))
                {
                    //simulate 2 players
                    int numGamePlayers = 2;
                    //set card travel speed
                    float cardSpeed = 0.35f;
                    for (int k = 0; k < numGamePlayers; k++) 
                    {
                        cardModel[k].StartFly(k, cardSpeed, numGamePlayers);
                        cardsInPlay++;
                    }
                    for (int i = 0; i < numGamePlayers; i++)
                    {
                        cardModel[i+numGamePlayers].StartFly(i + numGamePlayers, cardSpeed, numGamePlayers);
                        cardsInPlay++;
                    }
                    // call the following method when the game state is active, need matchmaking to work first:
                    //parseMyCards();
                    //Flip player's card for them to see
                    int playerSeatPlaceholder = 1;
                    //the following two lines are for when the matchmaking succeeds and the game state is pulled:
                    int cardIndex0 = 2;//myCards[0];
                    int cardIndex1 = 15;//myCards[1];
                                        // if we want to see what the cards are, we can uncomment the following two lines:
                                        //print(cardIndex0);
                                        //print(cardIndex1);
                                        //flip the two player cards for them to see

                    cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder].cardBackOrig,
                        cardModel[playerSeatPlaceholder].cardFaces[cardIndex0], playerSeatPlaceholder, numGamePlayers, cardSpeed);

                    cardModel[playerSeatPlaceholder + numGamePlayers].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder + numGamePlayers].cardBackOrig,
                        cardModel[playerSeatPlaceholder + numGamePlayers].cardFaces[cardIndex1], playerSeatPlaceholder + numGamePlayers, numGamePlayers, cardSpeed);
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
            }*/

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
