using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    public Sprite[] cardFaces;
    

    //Locations for each player spot at the table
    private Vector3 pos0 = new Vector3(6, 2, 0);
    private Vector3 pos1 = new Vector3(7, 0, 0);
    private Vector3 pos2 = new Vector3(6, -2, 0);
    private Vector3 pos3 = new Vector3(3, -2, 0);
    private Vector3 pos4 = new Vector3(-3, -2, 0);
    private Vector3 pos5 = new Vector3(-6, -2, 0);
    private Vector3 pos6 = new Vector3(-7, 0, 0);
    private Vector3 pos7 = new Vector3(-6, 2, 0);

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

    public void StartFly(int dealNum, float speed)
    {
        //If on second round of cads, place to the side of already dealt card
        if (dealNum < 8)
        {
            Vector3 dest = getPos(dealNum);
            dest.x = dest.x + 1f;
            StartCoroutine(LerpRoutine(dest, speed, (dealNum + 1) * speed));
        }
        else
        {
            StartCoroutine(LerpRoutine(getPos(dealNum - 8), speed, (dealNum + 1) * speed));
        }
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

    private Vector3 getPos(int i)
    {
        if (i == 0)
        {
            return this.pos0;
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
