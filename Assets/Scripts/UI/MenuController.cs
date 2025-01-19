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

    private void Awake()
    {
        start = GameObject.Find("StartBtn").GetComponent<Button>();
        gameIll = GameObject.Find("GameIllBtn").GetComponent<Button>();
        propsIll = GameObject.Find("PropsIllBtn").GetComponent<Button>();

        titleScreen = GameObject.Find("TitleScreen");
        gameIllImg = GameObject.Find("GameIllustration");
        propsIllImg = GameObject.Find("PropsIllustration");
    }

    void Start()
    {
        start.onClick.AddListener(StartGame);
        gameIll.onClick.AddListener(ShowGameIll);
        propsIll.onClick.AddListener(ShowPropsIll);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("WaitingRoom");
    }

    private void ShowGameIll()
    {
        titleScreen.SetActive(false);
        gameIllImg.SetActive(true);
    }

    private void ShowPropsIll()
    {
        titleScreen.SetActive(false);
        propsIllImg.SetActive(true);
    }
}
