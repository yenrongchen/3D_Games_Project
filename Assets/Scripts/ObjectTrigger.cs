using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class ObjectTrigger : MonoBehaviour
{
    public Flowchart flowchart;
    private bool isIn = false;

    // Start is called before the first frame update
    void Start()
    {
        isIn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isIn)  // 确保只有在没有正在执行时才处理点击
        {
            Saysomething();
        }
    }

    private void Saysomething()
    {
        string blockName = this.gameObject.tag;  // 使用物件的 Tag 名称作为 Block 名称
        // 调用指定的 Fungus Block
        if (flowchart != null && !string.IsNullOrEmpty(blockName))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Flowchart.BroadcastFungusMessage(blockName);
            }
        }
        else
        {
            Debug.LogWarning($"未能找到 Flowchart 或 Block 名称无效！物件: {gameObject.name}");
        }
    }

    public void OnPlayerTriggerEnter()
    {
        isIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
 
            isIn = false;
        
    }
}
