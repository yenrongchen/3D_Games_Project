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
