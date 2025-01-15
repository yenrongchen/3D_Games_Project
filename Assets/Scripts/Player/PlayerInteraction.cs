using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private FadeInOut fadeInOut;
    private FirstPersonController player;

    private void Start()
    {
        fadeInOut = GameObject.Find("GameManager").GetComponent<FadeInOut>();
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
        // pause player control
        player.DisableMovement();

        // fade in
        fadeInOut.FadeIn();
        float fadeTime = fadeInOut.getTimeToFade();
        yield return new WaitForSeconds(fadeTime);

        // teleport
        player.Teleport(targetPosition);

        // fade out
        fadeInOut.FadeOut();
        yield return new WaitForSeconds(fadeTime);

        // resume player control
        player.EnableMovement();
    }
}
