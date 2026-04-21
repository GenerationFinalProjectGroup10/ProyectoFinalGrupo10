using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float duration = 1f;

    void Start()
    {
        StartCoroutine(Fade());
    }

    System.Collections.IEnumerator Fade()
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = time / duration;
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}