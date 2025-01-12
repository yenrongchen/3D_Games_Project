using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isPaused = false;
    private int nextLevel = 1;
    // Start is called before the first frame update
    void Start()
    {
        //int nextLevel = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Time.timeScale = 0; // pause the game
                isPaused = true;
                // TODO: show menu
            }
            else
            {
                Time.timeScale = 1;
                isPaused = false;
            }
        }
    }

    public bool getIsPaused()
    {
        return isPaused;
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
