using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerButtonAssign : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    //Locations for each player spot at the table
    private Vector3 pos0 = new Vector3(2.52f, 2.8f, 0);
    private Vector3 pos1 = new Vector3(3.32f, 0.64f, 0);
    private Vector3 pos2 = new Vector3(2.49f, -0.95f, 0);
    private Vector3 pos3 = new Vector3(-2.44f, -0.93f, 0);
    private Vector3 pos4 = new Vector3(-5.42f, -0.93f, 0);
    private Vector3 pos5 = new Vector3(-5.3f, 0.56f, 0);
    private Vector3 pos6 = new Vector3(-4.94f, 2.86f, 0);
    private Vector3 pos7 = new Vector3(-0.39f, 2.93f, 0);

    //Save the original location & card back for use in card recall
    private Vector3 home;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void Start()
    {
        //save the starting location
        home = transform.position;
    }

    public void StartFly(int playerNum)
    {
        //If on second round of cads, place to the side of already dealt card
        Vector3 dest = getPos(playerNum);
        dest.x = dest.x + 1f;
        StartCoroutine(LerpRoutine(dest, 0.35f));
    }

    private IEnumerator LerpRoutine(Vector3 target, float duration)
    {
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
