using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image progressImage;

    [SerializeField]
    private TextMeshProUGUI countdownText;

    [SerializeField]
    private float totalDuration;

    [SerializeField]
    private UnityEvent onCompleted;

    private Coroutine animationCoroutine;

    public void StartCounterCountdown(float duration)
    {
        totalDuration = duration;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(CounterCountdown());
    }

    private IEnumerator CounterCountdown()
    {
        float remainingTime = totalDuration;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Clamp(remainingTime, 0, totalDuration);

            progressImage.fillAmount = 1 - remainingTime / totalDuration;
            countdownText.text = remainingTime.ToString("F1") + "s";

            yield return null;
        }

        progressImage.fillAmount = 0;
        if (countdownText != null)
        {
            countdownText.text = "0s";
        }

        onCompleted?.Invoke();
    }

    public void StartCountdown(float duration)
    {
        totalDuration = duration;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        float remainingTime = totalDuration;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Clamp(remainingTime, 0, totalDuration);

            progressImage.fillAmount = remainingTime / totalDuration;
            countdownText.text = remainingTime.ToString("F1") + "s";

            yield return null;
        }

        progressImage.fillAmount = 0;
        if (countdownText != null)
        {
            countdownText.text = "0s";
        }

        onCompleted?.Invoke();
    }
}
