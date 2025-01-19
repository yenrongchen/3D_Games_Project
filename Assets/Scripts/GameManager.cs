using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int currentLevel;

    public Item lv1Gem;
    public Item lv2Gem;
    public Item lv3Gem;

    private bool isPaused = false;

    private FadeInOut fadeInOut;
    private FirstPersonController player;

    private GameObject pauseCanvas;
    private GameObject deathCanvas;

    // Start is called before the first frame update
    void Start()
    {
        GameObject LvPanel = GameObject.Find("LevelText");
        if (LvPanel != null)
        {
            TextMeshProUGUI lvText = LvPanel.GetComponent<TextMeshProUGUI>();
            lvText.text = "LEVEL " + currentLevel.ToString();
        }

        HideCursor();

        player = GameObject.Find("Player").GetComponent<FirstPersonController>();

        fadeInOut = GameObject.Find("FadeInOutCanvas").GetComponent<FadeInOut>();

        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);

        deathCanvas = GameObject.Find("DeathCanvas");
        deathCanvas.SetActive(false);

        if (currentLevel > 1) CheckGems();
    }

    // Update is called once per frame
    void Update()
    {
        // pause or resume the game
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isPaused)
            {
                Time.timeScale = 0;
                isPaused = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pauseCanvas.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;
                HideCursor();
                pauseCanvas.SetActive(false);
            }
        }
    }

    public void EnterRoom()
    {
        // for initial dialogue
        if (currentLevel == 3)
        {
            PlayerPrefs.SetInt("times", 2);
        }
        else
        {
            PlayerPrefs.SetInt("times", 1);
        }

        // for sleep
        PlayerPrefs.SetInt("status", 1);

        // for level
        PlayerPrefs.SetInt("level", currentLevel);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene("WaitingRoom");
    }

    public void EnterMaze()
    {
        int targetLevel = PlayerPrefs.GetInt("level", 0);

        if (targetLevel == 0)
        {
            SceneManager.LoadScene("Level 1");
        }
        else if (targetLevel == 1)
        {
            int gem1 = PlayerPrefs.GetInt("Lv1Gem", 0);
            if (gem1 == 0)
            {
                // not get gem, go to hard map
                SceneManager.LoadScene("Level 2");
            }
            else
            {
                SceneManager.LoadScene("Level 2-e");
            }
        }
        else
        {
            int gem2 = PlayerPrefs.GetInt("Lv2Gem", 0);
            if (gem2 == 0)
            {
                // not get gem, go to hard map
                SceneManager.LoadScene("Level 3");
            }
            else
            {
                SceneManager.LoadScene("Level 3-e");
            }
        }
    }

    public void FadeInOutForSleep()
    {
        StartCoroutine(FadeForSleep());
    }

    private IEnumerator FadeForSleep()
    {
        player.Freeze();
        fadeInOut.setTimeToFade(1.5f);

        fadeInOut.FadeIn();
        yield return new WaitForSeconds(1.5f);

        fadeInOut.FadeOut();
        yield return new WaitForSeconds(1.5f);

        player.Unfreeze();
    }

    public void EnterMazeWithFade()
    {
        StartCoroutine(FadeForMaze());
    }

    private IEnumerator FadeForMaze()
    {
        player.Freeze();
        fadeInOut.setTimeToFade(1.25f);

        fadeInOut.FadeIn();
        yield return new WaitForSeconds(1.25f);

        EnterMaze();
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CheckGems()
    {
        if (PlayerPrefs.GetInt("Lv1Gem", 0) != 0)
        {
            InventoryManager.Instance.Add(lv1Gem);
        }
        if (PlayerPrefs.GetInt("Lv2Gem", 0) != 0)
        {
            InventoryManager.Instance.Add(lv2Gem);
        }
    }

    public bool GetPaused()
    {
        return isPaused;
    }

    public void GameOver()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        deathCanvas.SetActive(true);
        player.Freeze();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClearGameData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
