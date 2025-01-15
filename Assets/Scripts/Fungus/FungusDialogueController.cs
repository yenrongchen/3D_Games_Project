using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class FungusController : MonoBehaviour
{
    private FirstPersonController playerController;

    void Start()
    {
        // 获取玩家控制脚本
        playerController = FindObjectOfType<FirstPersonController>();
    }

    private void Update()
    {
        var flowchart = FindObjectOfType<Flowchart>();
        if (flowchart.HasExecutingBlocks()) {
            OnDialogueStarted();
        }
        else
        {
            OnDialogueEnded();
        }
    }

    void OnDialogueStarted()
    {
        // 禁用玩家移动
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    void OnDialogueEnded()
    {
        // 启用玩家移动
        if (playerController != null)
        {
            playerController.enabled=true;
        }
    }


}
