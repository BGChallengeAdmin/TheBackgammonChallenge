using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInFadeOutImage : MonoBehaviour
{
    [SerializeField] Image _fadeImage;

    private Color blackColour = Color.black;

    internal void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }

    internal void SetBlackAlphaValue(float percent)
    {
        blackColour.a = percent;
        _fadeImage.color = blackColour;
    }

    internal void SetFadeToBlackSeconds(float timeDelay)
    {
        StartCoroutine(FadeToBlackCoroutine(timeDelay));
    }

    private IEnumerator FadeToBlackCoroutine(float timeDelay)
    {
        SetBlackAlphaValue(0f);

        var percent = 0f;
        var startTime = System.DateTime.Now;
        var delay = timeDelay * 1000f;

        while (percent < 100)
        {
            var delta = (System.DateTime.Now - startTime).TotalMilliseconds;
            percent = Mathf.Clamp((float)(delta * 100f / delay), 0f, 100f);

            SetBlackAlphaValue(percent * .01f);

            yield return null;
        }
    }

    internal void SetFadeOutFromBlackSeconds(float timeDelay)
    {
        if (this.gameObject.activeInHierarchy)
            StartCoroutine(FadeFromBlackCoroutine(timeDelay));
    }

    private IEnumerator FadeFromBlackCoroutine(float timeDelay)
    {
        var percent = 100f;
        var startTime = System.DateTime.Now;
        var delay = timeDelay * 1000f;

        while (percent > 0)
        {
            var delta = (System.DateTime.Now - startTime).TotalMilliseconds;
            percent = 100f - Mathf.Clamp((float)(delta * 100f / delay), 0f, 100f);

            SetBlackAlphaValue(percent * .01f);

            yield return null;
        }

        // RESET BACK TO BLACK
        SetBlackAlphaValue(1f);
        SetActive(false);
    }
}
