using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class countdownTimer : MonoBehaviour
{
    //where player number = i:
    // the time unitl their bet is (i*30)-30
    // every 30 seconds the game state should fetch the most recent bets
    // one time is 0, they have the option to place their bet
    // we could constantly check for a plaer to make a bet to proceed ahead of the 30 second interval 
    // we use 5 seconds here for testing purposes, so we change our time equation to (i*5)-4 
    // we use -4 to add 1 second of buffer time for everything to catch up

    float timeLeft = 30;

    private static GameObject text;
    private static Text time;
    private static bool toggle = false;

    private void Awake()
    {
        Debug.Log("TIMER WAKE");
        text = GameObject.Find("Timer");
        time = text.GetComponent<Text>();
    }

    void Update()
    {
        // reset the timer
        if (toggle)
        {
            timeLeft = 30;
        }

        timeLeft -= (Time.deltaTime * 0.10f);
        if (GlobalVars.isTurn)
        {
            toggle = false;
            // start counting down
            if (Mathf.Round(timeLeft) == 30)
            {
                //BBP1.IsActive().Equals(true);
                //BBM1.IsActive().Equals(true);
                //PlaceBet.IsActive().Equals(true);
                //BetSlider.IsActive().Equals(true);
            }
            if (0 < Mathf.Floor(timeLeft) && Mathf.Round(timeLeft) <= 30)
            {
                time.text = ("Time Left:" + Mathf.Round(timeLeft));
            }
            if (Mathf.Round(timeLeft) == 0)
            {
                time.text = "Time Left:" + Mathf.Round(timeLeft);
                //BBP1.IsActive().Equals(false);
                //BBM1.IsActive().Equals(false);
                //PlaceBet.IsActive().Equals(false);
                //BetSlider.IsActive().Equals(false);
                timeLeft = 30; // reset timer
                GlobalVars.isTurn = false;
                GlobalVars.AFK++;
            }
            timeLeft -= Time.deltaTime;
        }
        else
        {
            toggle = true;
            time.text = "";
        }
    }
}
