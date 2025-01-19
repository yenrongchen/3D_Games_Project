using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathController : MonoBehaviour
{
    private Button againBtn;
    private Button mainMenuBtn;

    private FadeInOut fadeInOut;

    // Start is called before the first frame update
    void Start()
    {
        againBtn = GameObject.Find("PlayAgainBtn").GetComponent<Button>();
        mainMenuBtn = GameObject.Find("DMainMenuBtn").GetComponent<Button>();

        againBtn.onClick.AddListener(PlayAgain);
        mainMenuBtn.onClick.AddListener(MainMenu);

        fadeInOut = GameObject.Find("FadeInOutCanvas").GetComponent<FadeInOut>();
    }

    private void PlayAgain()
    {
        StartCoroutine(GoTo("WaitingRoom"));
    }

    private void MainMenu()
    {
        StartCoroutine(GoTo("Menu"));
    }

    private IEnumerator GoTo(string sceneName)
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();

        GameObject.Find("FadeInOutCanvas").GetComponent<Canvas>().sortingOrder = 5;

        fadeInOut.setTimeToFade(1f);

        fadeInOut.FadeIn();
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    }
}
