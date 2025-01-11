using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Portal"))
        {
            if (collision.transform.GetComponent<PortalController>().checkCanTP())
            {
                Vector3 targetPosition = collision.transform.GetComponent<PortalController>().getTeleportPos();
                GameObject.Find("Player").GetComponent<FirstPersonController>().teleport(targetPosition);
            }
        }
    }
}
