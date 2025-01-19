using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private Button start;
    private Button gameIll;
    private Button propsIll;

    private GameObject titleScreen;
    private GameObject gameIllImg;
    private GameObject propsIllImg;

    private FadeInOut fadeInOut;

    private void Awake()
    {
        start = GameObject.Find("StartBtn").GetComponent<Button>();
        gameIll = GameObject.Find("GameIllBtn").GetComponent<Button>();
        propsIll = GameObject.Find("PropsIllBtn").GetComponent<Button>();

        titleScreen = GameObject.Find("TitleScreen");
        gameIllImg = GameObject.Find("GameIllustration");
        propsIllImg = GameObject.Find("PropsIllustration");

        fadeInOut = GameObject.Find("FadeInOutCanvas").GetComponent<FadeInOut>();
    }

    void Start()
    {
        start.onClick.AddListener(StartGame);
        gameIll.onClick.AddListener(ShowGameIll);
        propsIll.onClick.AddListener(ShowPropsIll);
    }

    private void StartGame()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
        StartCoroutine(StartGameWithFade());
    }

    private IEnumerator StartGameWithFade()
    {
        fadeInOut.setTimeToFade(1.25f);

        fadeInOut.FadeIn();
        yield return new WaitForSeconds(1.25f);

        SceneManager.LoadScene("WaitingRoom");
    }

    private void ShowGameIll()
    {
        titleScreen.SetActive(false);
        propsIllImg.SetActive(false);
        gameIllImg.SetActive(true);
    }

    private void ShowPropsIll()
    {
        titleScreen.SetActive(false);
        gameIllImg.SetActive(false);
        propsIllImg.SetActive(true);
    }
}
