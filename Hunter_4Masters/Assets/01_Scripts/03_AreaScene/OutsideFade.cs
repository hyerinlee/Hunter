using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideFade : MonoBehaviour
{
    private SpriteRenderer outsideImg;
    private Coroutine outsideCoroutine;

    private void Awake()
    {
        outsideImg = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            if (outsideCoroutine != null) StopCoroutine(outsideCoroutine);
            outsideCoroutine = StartCoroutine(OutsideImgFade(0));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            if (outsideCoroutine != null) StopCoroutine(outsideCoroutine);
            outsideCoroutine = StartCoroutine(OutsideImgFade(1));
        }
    }

    IEnumerator OutsideImgFade(int to)
    {
        float from = outsideImg.color.a;
        float delay = 0.0f;
        Color color = outsideImg.color;

        while (delay < 0.5f)
        {
            color.a += (to - from) * Time.deltaTime * 2;
            outsideImg.color = color;
            delay += Time.deltaTime;
            yield return null;
        }
    }
}
