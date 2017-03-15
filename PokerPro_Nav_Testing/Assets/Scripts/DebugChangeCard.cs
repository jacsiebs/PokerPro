using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChangeCard : MonoBehaviour
{
    CardModel[] cardModel = new CardModel[25];
    //we want to keep track of how many cards are in play so we know how many to recall
    private int cardsInPlay = 0;
    void Start()
    {
        /*
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }*/
    }

    void Awake()
    {
        List<GameObject> cards = new List<GameObject>();
        for (int k = 25; k > -1; k--)
        {
            cards.Add(GameObject.Find("Card" + k.ToString()));
            
        }

        for (int k = 0; k < 25; k++)
        {
            cardModel[k] = cards[k].GetComponent<CardModel>();
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 175, 28), "Deal from deck: 8 Players"))
        {
            //TODO: Get num of players and deal cards accordingly
            //substitute 'numGamePlayers' for the value returned by getPlayers() or similar
            //set card travel speed
            //simulate 8 players
            int numGamePlayers = 8;
            float cardSpeed = 0.35f;
            for (int i = 0; i < (2*numGamePlayers); i++)
            {
                cardModel[i].StartFly(i, cardSpeed);
                cardsInPlay++;
            }
            //Flip player's card for them to see
            int playerSeatPlaceholder = 1;
            //Simulate deal from shuffle deck
            int cardIndex0 = Random.Range(0, 51);
            int cardIndex1 = cardIndex0;
            //check for no duplicates
            while (cardIndex1 == cardIndex0)
            {
                cardIndex1 = Random.Range(0, 51);
            }
            
            //CardFlipper tst = cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>();
            cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder].cardBackOrig, 
                cardModel[playerSeatPlaceholder].cardFaces[cardIndex0], playerSeatPlaceholder, numGamePlayers, cardSpeed);
            cardModel[playerSeatPlaceholder+8].GetComponent<CardFlipper>().FlipCard(cardModel[playerSeatPlaceholder+8].cardBackOrig, 
                cardModel[playerSeatPlaceholder+8].cardFaces[cardIndex1], playerSeatPlaceholder+8, numGamePlayers, cardSpeed);
        }

        if(GUI.Button(new Rect(10, 50, 120, 28), "Recall cards"))
        {
            float cardSpeed = 0.35f;
            for (int i = 0; i < cardsInPlay; i++)
            {
                cardModel[i].StartRecall(cardSpeed);
            }
        }

        if (GUI.Button(new Rect(10, 90, 180, 28), "Simulate fold for Player 6"))
        {
            //Player 6 will have cardModels[6] and cardModels[14]
            //this could be derrived from a .getPlayer() method or similar
            cardModel[6].PlayerFold();
            cardModel[14].PlayerFold();
        }
        if (GUI.Button(new Rect(10, 130, 210, 28), "Simulate show for Players 0, 3, 6"))
        {
            for (int i = 0; i < 8; i+=3)
            {
                float cardSpeed = 0.35f;
                //Flip player's card for them to see
                int playerSeatPlaceholder = i;
                //Simulate deal from shuffle deck
                int cardIndex0 = Random.Range(0, 51);
                int cardIndex1 = cardIndex0;
                //check for no duplicates
                while (cardIndex1 == cardIndex0)
                {
                    cardIndex1 = Random.Range(0, 51);
                }
                //we want to show right away, so set this to 0 for no delay
                int numGamePlayers = 0;
                //CardFlipper tst = cardModel[playerSeatPlaceholder].GetComponent<CardFlipper>();
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
                //Simulate deal from shuffle deck, we don't check for duplicates because that's too much work for this test
                int cardIndex = Random.Range(0, 51);
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
                //Simulate deal from shuffle deck, we don't check for duplicates because that's too much work for this test
                int cardIndex = Random.Range(0, 51);
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
                //Simulate deal from shuffle deck, we don't check for duplicates because that's too much work for this test
                int cardIndex = Random.Range(0, 51);
                //we want to show right away, so set this to 0 for no delay
                cardModel[2 * numGamePlayers + i].GetComponent<CardFlipper>().FlipCard(cardModel[2 * numGamePlayers + i].cardBackOrig,
                    cardModel[2 * numGamePlayers + i].cardFaces[cardIndex], (2 * numGamePlayers + i), 0, cardSpeed);
            }
        }

    }

}
