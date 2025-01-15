using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public Image hpBar;

    private float hp;
    private float maxHP;
    private float lerpSpeed;

    // Start is called before the first frame update
    void Start()
    {
        maxHP = GameObject.Find("Player").GetComponent<FirstPersonController>().getMaxHP();
        hp = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        hp = GameObject.Find("Player").GetComponent<FirstPersonController>().getCurrentHP();
        hpText.text = "HP: " + hp.ToString();

        lerpSpeed = 5f * Time.deltaTime;

        hpBarFiller();
    }

    private void hpBarFiller()
    {
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hp / maxHP, lerpSpeed);
    }
}
