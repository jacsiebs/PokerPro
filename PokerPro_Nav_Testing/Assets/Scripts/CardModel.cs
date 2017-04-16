using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    public Sprite[] cardFaces;
    

    //Locations for each player spot at the table
    private Vector3 pos0 = new Vector3(3.8f, 2f, 0);
    private Vector3 pos1 = new Vector3(4.75f, -0.45f, 0);
    private Vector3 pos2 = new Vector3(3.8f, -2.75f, 0);
    private Vector3 pos3 = new Vector3(0.35f, -2.85f, 0);
    private Vector3 pos4 = new Vector3(-3.15f, -2.85f, 0);
    private Vector3 pos5 = new Vector3(-6.7f, -2.65f, 0);
    private Vector3 pos6 = new Vector3(-7.2f, 0, 0);
    private Vector3 pos7 = new Vector3(-6.15f, 2f, 0);

    //Locations for the flop/river cards
    private Vector3 posA = new Vector3(-1, 0, 0);
    private Vector3 posB = new Vector3(0, 0, 0);
    private Vector3 posC = new Vector3(1, 0, 0);
    private Vector3 posD = new Vector3(2, 0, 0);
    private Vector3 posE = new Vector3(3, 0, 0);

    //Save the original location & card back for use in card recall
    private Vector3 home;
    public Sprite cardBackOrig;
    public Sprite cardBack;
    public bool isFolded = false;

    public int cardIndex;

    public void toggleFace(bool showFace)
    {


        if (showFace)
        {
            spriteRenderer.sprite = cardFaces[cardIndex];
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }

    }
    ////////////
    private void Start()
    {
        //save the starting location & card back sprite
        home = transform.position;
        cardBackOrig = spriteRenderer.sprite;
    }

    public void StartFly(int dealNum, float speed, int numGamePlayers)
    {
        //If on second round of cads, place to the side of already dealt card
        if (dealNum < numGamePlayers)
        {
            Vector3 dest = getPos(dealNum);
            dest.x = dest.x + 1f;
            StartCoroutine(LerpRoutine(dest, speed, (dealNum + 1) * speed));
        }
        else
        {
            StartCoroutine(LerpRoutine(getPos(dealNum - numGamePlayers), speed, (dealNum + 1) * speed));
        }
    }

    public void StartRiverDeal(int alpha, float speed)
    {
        StartCoroutine(LerpRoutine(getPosRiver(alpha), speed, speed));
    }

    public void StartRecall(float speed)
    {
        StartCoroutine(LerpRoutine(home, speed, speed));
        spriteRenderer.sprite = cardBackOrig;
    }

    public void PlayerFold()
    {
        spriteRenderer.sprite = cardBack;
        isFolded = true;
    }

    private IEnumerator LerpRoutine(Vector3 target, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 start = transform.position;
        float elapsedTime = 0.0f;

        while (transform.position != target)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsedTime / duration);
            yield return null;
        }
    }
    /////////////

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private Vector3 getPosRiver(int alpha)
    {
        if (alpha == 0)
        {
            return posA;
        }
        if (alpha == 1)
        {
            return posB;
        }
        if (alpha == 2)
        {
            return posC;
        }
        if (alpha == 3)
        {
            return posD;
        }
        if (alpha == 4)
        {
            return posE;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    private Vector3 getPos(int i)
    {
        if (i == 0)
        {
            return pos0;
        }
        if (i == 1)
        {
            return pos1;
        }
        if (i == 2)
        {
            return pos2;
        }
        if (i == 3)
        {
            return pos3;
        }
        if (i == 4)
        {
            return pos4;
        }
        if (i == 5)
        {
            return pos5;
        }
        if (i == 6)
        {
            return pos6;
        }
        if (i == 7)
        {
            return pos7;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

}
