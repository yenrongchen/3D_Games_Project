using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    private Button restartBtn;
    private Button mainMenuBtn;

    void Start()
    {
        restartBtn = GameObject.Find("RestartBtn").GetComponent<Button>();
        mainMenuBtn = GameObject.Find("MainMenuBtn").GetComponent<Button>();

        restartBtn.onClick.AddListener(Restart);
        mainMenuBtn.onClick.AddListener(MainMenu);
    }

    private void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void MainMenu()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
}
