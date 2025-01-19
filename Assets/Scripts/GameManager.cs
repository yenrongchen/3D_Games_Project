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
    private string nextLevel = "Level 1"; // to be fixed!!

    private FadeInOut fadeInOut;
    private FirstPersonController player;

    private GameObject pauseCanvas;

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

        fadeInOut = GetComponent<FadeInOut>();
        StartCoroutine(FadeOut());

        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);

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

    public void EnterMaze()
    {
        // which level?
    }

    public void EnterRoom()
    {
        if (currentLevel == 3)
        {
            PlayerPrefs.SetInt("times", 2);
        }
        else
        {
            PlayerPrefs.SetInt("times", 1);
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // record gems collected

        SceneManager.LoadScene("WaitingRoom");
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

        // need fix
        SceneManager.LoadScene(nextLevel);
    }

    private IEnumerator FadeOut()
    {
        player.DisableMovement();

        fadeInOut.FadeOut();
        yield return new WaitForSeconds(1.25f);

        player.EnableMovement();
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
