using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fungus : MonoBehaviour
{
    private FirstPersonController playerController;

    void Start()
    {
        // 获取玩家控制脚本
        playerController = FindObjectOfType<FirstPersonController>();

        // 订阅 Fungus 的对话事件
        FungusManager.Instance.DialogueStarted += OnDialogueStarted;
        FungusManager.Instance.DialogueEnded += OnDialogueEnded;
    }

    void OnDialogueStarted()
    {
        // 禁用玩家移动
        if (playerController != null)
        {
            playerController.DisableMovement();
        }
    }

    void OnDialogueEnded()
    {
        // 启用玩家移动
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }

    void OnDestroy()
    {
        // 取消订阅事件
        FungusManager.Instance.DialogueStarted -= OnDialogueStarted;
        FungusManager.Instance.DialogueEnded -= OnDialogueEnded;
    }
}
