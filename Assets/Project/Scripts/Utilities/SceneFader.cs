using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage;

    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;

    void Start()
    {
        // iniciar negro
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 1);

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / fadeInDuration;

            if (fadeImage != null)
                fadeImage.color = new Color(0, 0, 0, 1 - t);

            yield return null;
        }

        // 🔥 desactivar para no bloquear clicks
        fadeImage.gameObject.SetActive(false);
    }

    public void FadeToScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true); // 🔥 activar otra vez
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / fadeOutDuration;

            if (fadeImage != null)
                fadeImage.color = new Color(0, 0, 0, t);

            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}