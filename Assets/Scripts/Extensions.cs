using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class Extensions
{
    /// <summary>
    /// Shakes a UI panel over a specified duration and magnitude.
    /// </summary>
    /// <param name="rectTransform">The RectTransform to shake.</param>
    /// <param name="monoBehaviour">A MonoBehaviour to run the Coroutine on (e.g., 'this').</param>
    /// <param name="duration">How long the shake lasts in seconds.</param>
    /// <param name="magnitude">How violent the shake is.</param>
    public static void Shake(this RectTransform rectTransform, MonoBehaviour monoBehaviour, float duration, float magnitude)
    {
        monoBehaviour.StartCoroutine(ShakeCoroutine(rectTransform, duration, magnitude));
    }

    private static IEnumerator ShakeCoroutine(RectTransform rectTransform, float duration, float magnitude)
    {
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Calculate a random offset inside a unit circle
            Vector2 randomOffset = Random.insideUnitCircle * magnitude;

            // Apply the offset to the original position
            rectTransform.anchoredPosition = originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Reset precisely back to the original position when finished
        rectTransform.anchoredPosition = originalPosition;
    }

    public static void FailEffect(Volume volume, MonoBehaviour runner)
    {
        if (volume == null) return;

        // Try to get the Vignette component from the Volume Profile
        if (volume.profile.TryGet<Vignette>(out Vignette vignette))
        {
            // Save original settings to restore later
            Color originalColor = vignette.color.value;
            float originalIntensity = vignette.intensity.value;

            // 1. Instantly snap to a heavy red flash
            vignette.color.overrideState = true;
            vignette.intensity.overrideState = true;

            vignette.color.value = Color.red;
            vignette.intensity.value = 0.6f; // Make it noticeably intense

            // 2. Smoothly fade it back to normal over 2 seconds
            LeanTween.value(volume.gameObject, 0.6f, originalIntensity, 2f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate((float val) =>
                {
                    vignette.intensity.value = val;
                })
                .setOnComplete(() =>
                {
                    // Restore original color when done
                    vignette.color.value = originalColor;
                });
        }
    }

    public static void ZoomInOut(GameObject panel, float zoomLevel, float duration)
    {
        LeanTween.value(panel.gameObject, 1f, zoomLevel, duration)
            .setEaseInCubic()
            .setOnUpdate((float value) =>
            {
                panel.transform.localScale = new Vector3(value, value, value);
            })
            .setLoopPingPong(1);
    }

    /// <summary>
    /// Scale up or down a sprite by specified scale level over a given duration
    /// </summary>
    public static void ScaleUpDown(GameObject image, float from, float to, float duration)
    {
        if (from < to)
            image.SetActive(true);
        LeanTween.value(image, from, to, duration)
            .setEaseInOutCubic()
            .setOnUpdate((float value) =>
            {
                image.transform.localScale = new Vector3(value, value, value);
            })
            .setOnComplete(() =>
            {
                if (to == 0f)
                    image.SetActive(false);
            });
    }

    public static void OpacityFade(GameObject image, float from, float to, float duration)
    {
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = image.AddComponent<CanvasGroup>();
        }
        LeanTween.value(image, from, to, duration)
            .setEaseInOutCubic()
            .setOnUpdate((float value) =>
            {
                canvasGroup.alpha = value;
            })
            .setOnComplete(() =>
            {
                if (to <= 0.01f)
                {
                    image.SetActive(false);
                }
            });
    }
}