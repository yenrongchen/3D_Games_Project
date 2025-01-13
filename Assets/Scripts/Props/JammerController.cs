using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JammerController : MonoBehaviour
{
    private GameObject targetBarrier;

    public void setTargetBarrier(GameObject barrier)
    {
        targetBarrier = barrier;
    }

    public GameObject getTargetBarrier()
    {
        return targetBarrier;
    }
}
