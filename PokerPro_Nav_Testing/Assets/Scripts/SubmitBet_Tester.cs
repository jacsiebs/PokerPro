using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using LitJson;

//!only attached to the submit button
public class SubmitBet_Tester : MonoBehaviour
{
    // dependencies
    private static FakeSlider theSlider;// contains the slider script
    private static FakeServer server;
    static bool isFolded;
    static FakeGlobalVars globals;
    static FakeDisplayMessage messages;
    static FakeDisableButtons buttons;

    public static void setUp(FakeServer serv, FakeSlider slid, FakeGlobalVars globalHolder, FakeDisplayMessage messageHolder, FakeDisableButtons button_disabler, bool folded)
    {
        isFolded = folded;
        theSlider = slid;
        server = serv;
        globals = globalHolder;
        messages = messageHolder;
        buttons = button_disabler;
    }

    // use this function in on click
    public static void sendTheBet()
    {
        // sends the current bet on the slider
        betHelper();
    }

    // sumbit a bet equal to the bet currently before you
    public static void call()
    {
        // check if the player has enough chips
        // TODO replace with an all in
        if (GlobalVars.curr_bet > GlobalVars.chips)
        {
            Debug.Log("Not enough chips to call.");
           messages.print_message("Not enough chips to call");
        }
        // valid call
        else
        {
            turnOffSlider();
            GlobalVars.bet = GlobalVars.curr_bet;
            betHelper();
        }
    }

    // turns off the slider so that the bet can no longer be modified
    private static void turnOffSlider()
    {
        theSlider.interactable = false;
    }

    // turns on the slider so that the bet can be modified
    private static void turnOnSlider()
    {
        theSlider.interactable = true;
    }

    // submit a bet of all your chips
    // TODO handle the case where all in does not cover the call, currently it does not allow this
    public static void allIn()
    {
        turnOffSlider();
        GlobalVars.bet = GlobalVars.chips;
        betHelper();
    }

    // submit a fold
    public static void fold()
    {
        turnOffSlider();
        GlobalVars.bet = 0;// set the bet to 0 
        betHelper();
    }

    protected static void betHelper()
    {
        // check that the bet is valid - must be at least as large as the bet needed to call
        if (GlobalVars.bet < GlobalVars.curr_bet && !isFolded)
        {
            Debug.Log("Invalid bet. Your bet: " + GlobalVars.bet + "\nBet Needed: " + GlobalVars.curr_bet);
            messages.print_message("Submit a bet over " + GlobalVars.curr_bet + " or fold.");
            turnOnSlider();// renable if disabled by all in
        }

        // bet is valid or a fold - submit the bet and get the gamestate in return
        else
        {
            if (isFolded)
                Debug.Log("Sending Fold...");
            else
                Debug.Log("Sending bet...");

            turnOffSlider();// disable the bet slider

            // send the bet to the server
            string url = "http://104.131.99.193/game/" + globals.game_id + "/" + globals.player_id + "/" + globals.bet;
            server.send(url);
//            yield return www;

            // error check
            if (server.error != null)
            {
                Debug.Log("WWW submit bet error: " + server.error);
            }
            else
            {
                Debug.Log("Bet of " + globals.bet + " sent.");
                if (globals.bet == 0 && !isFolded)
                    messages.print_message("You have checked.");
                else if (isFolded)
                    messages.print_message("You have folded.");
                else
                    messages.print_message("You have bet " + globals.bet + ".");

                // update the gamestate - tested seperately

                // reset the bet needed to call
                globals.curr_bet = 0;

                // disable the bet UI
                buttons.disableButtons();
                theSlider.value = 0;// also makes GlobalVars.bet=0
                // continue to getupdatedgamestate()
            }          
        }
    }
}
