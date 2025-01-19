using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    private Button restartBtn;
    private Button mainMenuBtn;

    private FadeInOut fadeInOut;

    void Start()
    {
        restartBtn = GameObject.Find("RestartBtn").GetComponent<Button>();
        mainMenuBtn = GameObject.Find("MainMenuBtn").GetComponent<Button>();

        restartBtn.onClick.AddListener(Restart);
        mainMenuBtn.onClick.AddListener(MainMenu);

        fadeInOut = GameObject.Find("FadeInOutCanvas").GetComponent<FadeInOut>();
    }

    private void Restart()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void MainMenu()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        StartCoroutine(GoToMenu());
    }

    private IEnumerator GoToMenu()
    {
        fadeInOut.setTimeToFade(1f);

        fadeInOut.FadeIn();
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Menu");
    }
}
