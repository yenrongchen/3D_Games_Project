using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject gameOverUI;  // 連結你的 "Game Over" 菜單 UI
    private bool isGameOver = false;
    public bool getKey = false;
    // Start is called before the first frame update

    public void takeKey()
    {
        getKey = true;
    }
    void Start()
    {
        gameOverUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 測試用：按 "K" 模擬角色死亡
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerDied();
        }

        // 恢復遊戲，測試用
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            PlayerDied();
        }
    }

    public void PlayerDied()
    {
        if (isGameOver) return; // 確保只執行一次

        isGameOver = true;

        // 暫停遊戲
        Time.timeScale = 0;

        // 顯示死亡畫面
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 停用玩家控制
        DisablePlayerControls();
    }

    public void RestartGame()
    {
        // 恢復遊戲速度
        Time.timeScale = 1;

        // 隱藏死亡畫面
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // 重新加載場景
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void DisablePlayerControls()
    {
        // 停用玩家的移動或其他控制
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>(); // 替換為你的玩家控制腳本
            if (controller != null)
            {
                controller.enabled = false;
            }
        }
    }
}
