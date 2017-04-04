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

    float timeLeft = -1;

    private void Start()
    {
        getNum();
    }
    

    public Text text;
    public Button BBP1;
    public Button BBM1;
    public Button PlaceBet;
    public Slider BetSlider;

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft == 5)
        {
            BBP1.IsActive().Equals(true);
            BBM1.IsActive().Equals(true);
            PlaceBet.IsActive().Equals(true);
            BetSlider.IsActive().Equals(true);
        }
        if (0 < timeLeft && timeLeft < 5)
        {
            text.text = "Time Left:" + Mathf.Round(timeLeft);
        }
        if (timeLeft == 0)
        {
            text.text = "Time Left:" + Mathf.Round(timeLeft);
            BBP1.IsActive().Equals(false);
            BBM1.IsActive().Equals(false);
            PlaceBet.IsActive().Equals(false);
            BetSlider.IsActive().Equals(false);
            int resetTime = (5 * (DebugChangeCard.gameGlobals.numGamePlayers)) - 4;
            timeLeft = (float)resetTime;
        }
    }

    private IEnumerable getNum()
    {
        yield return 0;
        while (!DebugChangeCard.gameGlobals.isLoaded)
        {
            //wait to load  
            text.text = "Loading...";
        }
        int meVal = DebugChangeCard.gameGlobals.me;
        meVal = 5 * (meVal + 1) - 4;
        timeLeft = (float)meVal;
        print("MEVAL:" + meVal);
        text.text = "Time Left:" + timeLeft;
    }
}
