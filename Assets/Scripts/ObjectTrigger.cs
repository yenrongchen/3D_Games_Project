using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Fungus.FlowChart;

public class ObjectTrigger : MonoBehaviour
{
    //public Flowchart flowchart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPlayerTriggerEnter()
    {
        string blockName = gameObject.tag;  // 使用物件的 Tag 名称作为 Block 名称

        // 调用指定的 Fungus Block
        //if (flowchart != null && !string.IsNullOrEmpty(blockName))
        //{
        //    flowchart.ExecuteBlock(blockName);
        //}
        //else
        //{
        //    Debug.LogWarning($"未能找到 Flowchart 或 Block 名称无效！物件: {gameObject.name}");
        //}
    }
}
