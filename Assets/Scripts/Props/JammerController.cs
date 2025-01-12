using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JammerController : MonoBehaviour
{
    private GameObject targetBarrier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setTargetBarrier(GameObject barrier)
    {
        targetBarrier = barrier;
    }

    public GameObject getTargetBarrier()
    {
        return targetBarrier;
    }
}
