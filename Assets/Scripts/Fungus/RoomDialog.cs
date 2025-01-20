using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;

public class RoomDialog : MonoBehaviour
{
    private Camera mainCamera;
    private bool final = false;

    private GameObject good;
    private GameObject bad;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        int times = PlayerPrefs.GetInt("times", 0);
        if (times == 0)
        {
            Flowchart.BroadcastFungusMessage("Initial");
        }
        else if (times == 1)
        {
            Flowchart.BroadcastFungusMessage("Normal");
        }
        else 
        {
            final = true;
            Flowchart.BroadcastFungusMessage("Final");
        }

        good = GameObject.Find("good");
        good.SetActive(false);
        bad = GameObject.Find("bad");
        bad.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOrigin = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(rayOrigin, mainCamera.transform.forward, out RaycastHit hit, 80))
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                int status = PlayerPrefs.GetInt("status", 0);
                if (status == 0)
                {
                    Flowchart.BroadcastFungusMessage("CannotSleep");
                }
                else
                {
                    Flowchart.BroadcastFungusMessage("CanSleep");
                    PlayerPrefs.SetInt("status", 0);
                }
            }
        }
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("TableandChair"))
        {
            Flowchart.BroadcastFungusMessage("TableandChair");
        }

        if (collision.gameObject.CompareTag("CrateBox"))
        {
            Flowchart.BroadcastFungusMessage("CrateBox");
        }

        if (collision.gameObject.CompareTag("BeginDoor"))
        {
            if (final)
            {
                StartCoroutine(ShowEnding());
            }
            else
            {
                int sleepStatus = PlayerPrefs.GetInt("status", 0);
                if (sleepStatus == 0)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    GameObject.Find("Player").GetComponent<FirstPersonController>().Freeze();
                    Flowchart.BroadcastFungusMessage("BeginDoor");
                }
                else
                {
                    Flowchart.BroadcastFungusMessage("NeedSleep");
                }
            }
        }
    }

    private IEnumerator ShowEnding()
    {
        // fade in
        GameObject.Find("FadeInOutCanvas").GetComponent<FadeInOut>().setTimeToFade(1.2f);
        GameObject.Find("FadeInOutCanvas").GetComponent<FadeInOut>().FadeIn();
        yield return new WaitForSeconds(1.2f);

        // show ending
        int g1 = PlayerPrefs.GetInt("Lv1Gem", 0);
        int g2 = PlayerPrefs.GetInt("Lv2Gem", 0);
        int g3 = PlayerPrefs.GetInt("Lv3Gem", 0);
        int sum = g1 + g2 + g3;

        if (sum == 3)
        {
            good.SetActive(true);
            yield return new WaitForSeconds(2f);
            Flowchart.BroadcastFungusMessage("Good");
        }
        else
        {
            bad.SetActive(true);
            yield return new WaitForSeconds(2f);
            Flowchart.BroadcastFungusMessage("Bad");
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
