using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipper : MonoBehaviour {

    SpriteRenderer spriteRenderer;
    CardModel model;


    public AnimationCurve scaleCurve;
    public float duration = 0.5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        model = GetComponent<CardModel>();
    }

    public void FlipCard(Sprite startImage, Sprite endImage, int cardIndex, int numPlayers, float speed)
    {
        StopCoroutine(Flip(startImage, endImage, cardIndex, numPlayers, speed));
        StartCoroutine(Flip(startImage, endImage, cardIndex, numPlayers, speed));
    }

    IEnumerator Flip(Sprite startImage, Sprite endImage, int cardIndex, int numPlayers, float speed)
    {
        yield return new WaitForSeconds((2*speed) * numPlayers + 1f);
        if (model.isFolded)
        {
            spriteRenderer.sprite = model.cardBack;
        }
        else
        {
            spriteRenderer.sprite = startImage;
        }

        float time = 0f;
        while (time <= 1f)
        {
            float scale = scaleCurve.Evaluate(time);
            time = time + Time.deltaTime / duration;

            Vector3 localScale = transform.localScale;
            localScale.x = scale;
            transform.localScale = localScale;

            if (time >= 0.5f)
            {
                spriteRenderer.sprite = endImage;
            }

            yield return new WaitForFixedUpdate();
        }

        if (cardIndex == -1)
        {
            model.toggleFace(false);
        }

        /*else
        {
            model.cardIndex = cardIndex;
            model.toggleFace(true);
        }*/
    }
}
