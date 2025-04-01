using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeUIOut(float seconds)
    {
        StartCoroutine(FadeOut(seconds));
    }

    public void FadeUIIn(float seconds)
    {
        StartCoroutine(FadeIn(seconds));
    }

    IEnumerator FadeOut(float seconds)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1;
        while(canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / seconds;
            yield return null;
        }
        yield return null;
    }

    IEnumerator FadeIn(float seconds)
    {

        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / seconds;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return null;
    }

    void Update()
    {
        
    }
}
