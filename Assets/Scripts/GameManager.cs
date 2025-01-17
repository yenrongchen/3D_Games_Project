using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int currentLevel;

    private bool isPaused = false;
    private int nextLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI lvText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        lvText.text = "LEVEL " + currentLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isPaused)
            {
                // pause the game
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

    public void enterMaze()
    {
        SceneManager.LoadScene(nextLevel);
    }

    public void back()
    {
        SceneManager.LoadScene(0);
        nextLevel++;
    }
}
