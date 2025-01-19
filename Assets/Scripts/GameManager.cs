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
                // TODO: show menu
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;
                // TODO: hide menu
            }
        }
    }

    public void EnterMaze()
    {
        SceneManager.LoadScene(nextLevel);

        int gem1 = PlayerPrefs.GetInt("Lv1Gem", 0);
        if (gem1 == 1)
        {
            InventoryManager.Instance.Add(lv1Gem);
            PlayerPrefs.DeleteKey("Lv1Gem");
        }

        int gem2 = PlayerPrefs.GetInt("Lv2Gem", 0);
        if (gem2 == 1)
        {
            InventoryManager.Instance.Add(lv2Gem);
            PlayerPrefs.DeleteKey("Lv2Gem");
        }

        int gem3 = PlayerPrefs.GetInt("Lv3Gem", 0);
        if (gem3 == 1)
        {
            InventoryManager.Instance.Add(lv3Gem);
            PlayerPrefs.DeleteKey("Lv3Gem");
        }
        PlayerPrefs.Save();
    }

    public void EnterRoom()
    {
        PlayerPrefs.SetInt("status", 1);

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

        EnterMaze();
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
