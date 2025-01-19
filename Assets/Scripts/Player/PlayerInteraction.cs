using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private FadeInOut fadeInOut;
    private FirstPersonController player;
    private bool teleporting = false;

    private void Start()
    {
        fadeInOut = GameObject.Find("FadeInOutCanvas").GetComponent<FadeInOut>();
        player = GameObject.Find("Player").GetComponent<FirstPersonController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // teleport
        if (collision.transform.CompareTag("Portal") || collision.transform.CompareTag("PlacedPortal"))
        {
            if (collision.transform.GetComponent<PortalController>().CheckCanTP())
            {
                Vector3 targetPosition = collision.transform.GetComponent<PortalController>().getTeleportPos();
                StartCoroutine(TeleportWithFade(targetPosition));
            }
        }
    }

    private IEnumerator TeleportWithFade(Vector3 targetPosition)
    {
        teleporting = true;

        // pause player control
        player.DisableMovement();

        // fade in
        fadeInOut.setTimeToFade(0.8f);
        fadeInOut.FadeIn();
        yield return new WaitForSeconds(0.8f);

        // teleport
        player.Teleport(targetPosition);

        // fade out
        fadeInOut.FadeOut();
        yield return new WaitForSeconds(0.8f);

        // resume player control
        player.EnableMovement();

        teleporting = false;
    }

    public bool CheckTeleporting()
    {
        return teleporting;
    }
}
