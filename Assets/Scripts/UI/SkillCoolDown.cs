using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolDown : MonoBehaviour
{
    [SerializeField]
    private Image progressImage;

    private float totalDuration = 10f;

    private void Start()
    {
        totalDuration = GameObject.Find("Player").GetComponent<PlayerActionController>().jammerAtkCD;
    }

    public void Countdown(float remainingTime)
    {
        remainingTime = Mathf.Clamp(remainingTime, 0, totalDuration);
        progressImage.fillAmount = remainingTime / totalDuration;
    }
}
