using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject gameOverUI;  // �s���A�� "Game Over" ��� UI
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
        // ���եΡG�� "K" �������⦺�`
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerDied();
        }

        // ��_�C���A���ե�
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
        if (isGameOver) return; // �T�O�u����@��

        isGameOver = true;

        // �Ȱ��C��
        Time.timeScale = 0;

        // ��ܦ��`�e��
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // ���Ϊ��a����
        DisablePlayerControls();
    }

    public void RestartGame()
    {
        // ��_�C���t��
        Time.timeScale = 1;

        // ���æ��`�e��
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // ���s�[������
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void DisablePlayerControls()
    {
        // ���Ϊ��a�����ʩΨ�L����
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>(); // �������A�����a����}��
            if (controller != null)
            {
                controller.enabled = false;
            }
        }
    }
}
