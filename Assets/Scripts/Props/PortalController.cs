using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public Vector3 TeleportPosition;
    public GameObject PairPortal;
    private bool canTeleport = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Pre-defined teleport position
        if (TeleportPosition != Vector3.zero)
        {
            canTeleport = true;
        }
    }

    public void setTeleportPos(Vector3 position)
    {
        TeleportPosition = position;
        canTeleport = true;
    }

    public Vector3 getTeleportPos()
    {
        return TeleportPosition;
    }

    public bool CheckCanTP()
    {
        return canTeleport;
    }

    public void DisableTeleport()
    {
        canTeleport = false;
    }

    public void setPairPortal(GameObject pairPortal)
    {
        PairPortal = pairPortal;
    }

    public GameObject getPairPortal()
    {
        return PairPortal;
    }
}
