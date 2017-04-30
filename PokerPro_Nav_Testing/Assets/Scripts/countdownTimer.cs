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

    float timeLeft = 5;

    private static GameObject text;
    private static Text time;
    private Button BBP1;
    private Button BBM1;
    private Button PlaceBet;
    private Slider BetSlider;

    private void Awake()
    {
        Debug.Log("TIMER WAKE");
        text = GameObject.Find("Timer");
        time = text.GetComponent<Text>();
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (GlobalVars.isTurn || true)
        {
            Debug.Log("TIMER UPDATE");
            // start counting down
            if (Mathf.Round(timeLeft) == 5)
            {
                //BBP1.IsActive().Equals(true);
                //BBM1.IsActive().Equals(true);
                //PlaceBet.IsActive().Equals(true);
                //BetSlider.IsActive().Equals(true);
            }
            if (0 < Mathf.Round(timeLeft) && Mathf.Round(timeLeft) <= 5)
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
                timeLeft = 5; // reset timer
                GlobalVars.isTurn = false;
                GlobalVars.AFK++;
            }
            timeLeft -= Time.deltaTime;
            Debug.Log(timeLeft);
        }
        else
        {
            time.text = "";
        }
    }
}
