using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using TMPro;
using Fungus;
using StarterAssets;

public class PlayerActionController : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    [Header("Props")]
    public GameObject portalPrefab;
    public GameObject jammerPrefab;
    public GameObject jammerWithPartPrefab;
    public GameObject crosshairPrefab;
    public GameObject woodBoardPrefab;
    public GameObject almondWaterPrefab;
    public GameObject rationsPrefab;

    [Header("Jammer settings")]
    public float jammerAtkCD = 8f;

    [Header("UI")]
    public GameObject healingCanvas;

    // character controller
    private CharacterController controller;

    // props
    private List<string> showOutline = new() { "Props", "Jammer", "Key1", "Key2", "Key3", "PlacedJammer", "PlacedBoard" };
    private List<string> canPick = new() { "Props", "Jammer", "Key1", "Key2", "Key3" };

    // jammer
    private bool isHoldingJammer = false;
    private GameObject crosshair;
    private List<string> jammerTargetTags = new() { "Barrier", "Monster" };
    private float atkcd = 0f;

    // portal
    private bool isHoldingPortal = false;
    private GameObject previousPortal;
    private Vector3 previousPortalPos;
    private int portalCount = 0;

    // wood board
    private bool isHoldingBoard = false;

    // healer props
    private bool isHoldingAlmond = false;
    private bool isHoldingRations = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        crosshair = Instantiate(crosshairPrefab, Vector3.zero, Quaternion.identity);
        crosshair.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // clear outlines of all objects
        CancelAllOutlines();

        // raycast along with viewpoint
        RaycastHit hit;
        Vector3 rayOrigin = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(rayOrigin, mainCamera.transform.forward, out hit, 80))
        {
            // show props outlines
            ShowOutline(hit);

            // pick props
            if (Input.GetKey(KeyCode.F) && !isHoldingJammer && !isHoldingPortal && !isHoldingBoard)
            {
                PickProps(hit);
            }

            // retrieve props
            if (Input.GetKey(KeyCode.R) && !isHoldingJammer && !isHoldingPortal && !isHoldingBoard)
            {
                RetrieveProps(hit);
            }

            // jammer-specific action
            if (isHoldingJammer)
            {
                // show crosshair
                crosshair.SetActive(true);
                crosshair.transform.position = hit.point;

                // show outline of monsters and barrier when aiming to them
                if (jammerTargetTags.Contains(hit.transform.tag))
                {
                    var outline = hit.transform.gameObject.GetComponent<Outline>();
                    outline.enabled = true;
                }

                // attack cd count down
                if (atkcd > 0)
                {
                    atkcd -= Time.deltaTime;
                }

                // paralyze monster
                if (Input.GetKey(KeyCode.Mouse0) && hit.transform.CompareTag("Monster") && atkcd <= 0)
                {
                    GameObject monster = hit.transform.gameObject;
                    monster.GetComponent<MonsterController>().Paralyzed();
                    atkcd = jammerAtkCD;
                }
            }
            else
            {
                crosshair.SetActive(false);
            }
        }

        // place props
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            PlaceProps();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseProps();
        }

        // hold props
        if (Input.GetKeyDown(KeyCode.C))  // KEY BIND NEED FIX
        {
            HoldProps();
        }
    }

    private void CancelAllOutlines()
    {
        foreach (string tag in jammerTargetTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                var outline = obj.GetComponent<Outline>();
                outline.enabled = false;
            }
        }

        foreach (string tag in showOutline)
        {
            GameObject[] allProps = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject props in allProps)
            {
                var outline = props.GetComponent<Outline>();
                outline.enabled = false;
            }
        }

        if (GameObject.Find("OnHandProps") != null)
        {
            var outline = GameObject.Find("OnHandProps").GetComponent<Outline>();
            outline.enabled = false;
        }

        if (GameObject.Find("OnHandPortal") != null)
        {
            var outline = GameObject.Find("OnHandPortal").GetComponentInChildren<Outline>();
            outline.enabled = false;
        }

        GameObject[] allPortals = GameObject.FindGameObjectsWithTag("Portal");
        foreach (GameObject port in allPortals)
        {
            var outline = port.GetComponentInChildren<Outline>();
            outline.enabled = false;
        }

        GameObject[] allPlacedPortals = GameObject.FindGameObjectsWithTag("PlacedPortal");
        foreach (GameObject port in allPlacedPortals)
        {
            var outline = port.GetComponentInChildren<Outline>();
            outline.enabled = false;
        }
    }

    private void ShowOutline(RaycastHit hit)
    {
        if (showOutline.Contains(hit.transform.tag))
        {
            var outline = hit.transform.gameObject.GetComponent<Outline>();
            outline.enabled = true;
        }

        if (hit.transform.CompareTag("Portal") || hit.transform.CompareTag("PlacedPortal"))
        {
            var outline = hit.transform.gameObject.GetComponentInChildren<Outline>();
            outline.enabled = true;
        }
    }

    private void PickProps(RaycastHit hit)
    {
        // pick jammer
        // TODO: jammer inventory +1

        // pick portal
        if (hit.transform.CompareTag("Portal"))
        {
            GameObject pairPortal = hit.transform.gameObject.GetComponent<PortalController>().getPairPortal();
            if (pairPortal != null)
            {
                pairPortal.GetComponent<PortalController>().DisableTeleport();
            }

            // TODO: portal inventory +1
        }

        // pick wood board
        // TODO: board inventory +1

        // pick keys
        if (hit.transform.CompareTag("Key1") || hit.transform.CompareTag("Key2") || hit.transform.CompareTag("Key3"))
        {
            GameObject circlebase = GameObject.Find("CircleBase");
            Destroy(circlebase);

            // TODO: key inventory +1
        }

        // pick rations and almond water
        // TODO: inventory +1

        // pick gems
        // TODO: inventory +1

        // destroy object
        if (canPick.Contains(hit.transform.tag) || hit.transform.CompareTag("Portal"))
        {
            GameObject targetProps = hit.transform.gameObject;
            Destroy(targetProps);
        }
    }

    private void RetrieveProps(RaycastHit hit)
    {
        // retrieve jammer
        if (hit.transform.CompareTag("PlacedJammer"))
        { 
            GameObject jammer = hit.transform.gameObject;
            Destroy(jammer);

            // set barrier to visible
            GameObject barrier = jammer.GetComponent<JammerController>().getTargetBarrier();
            if (barrier != null)
            {
                barrier.GetComponent<BarrierController>().RemoveJammer();
            }

            // TODO: jammer inventory +1
        }

        // retrieve portal
        if (hit.transform.CompareTag("PlacedPortal"))
        {
            GameObject portal = hit.transform.gameObject;
            Destroy(portal);

            GameObject pairPortal = portal.GetComponent<PortalController>().getPairPortal();
            if (pairPortal != null)
            {
                pairPortal.GetComponent<PortalController>().DisableTeleport();
            }

            portalCount--;

            // TODO: portal inventory +1
        }

        // retrieve board
        if (hit.transform.CompareTag("PlacedBoard"))
        {
            GameObject targetBoard = hit.transform.gameObject;
            Destroy(targetBoard);
            // hole can fall
        }
    }

    private void HoldProps()
    {
        // TODO: How to determine which props to hold

        // hold jammer
        if (!isHoldingJammer)
        {
            GameObject jammer = Instantiate(jammerPrefab, mainCamera.transform);

            // position (right-bottom corner of the screen)
            Vector3 screenPosition = new(Screen.width * 0.8f, Screen.height * -0.25f, 0.8f);
            Vector3 jammerPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            jammer.transform.position = jammerPosition;

            // rotation
            Vector3 playerRot = this.transform.rotation.eulerAngles;
            Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
            jammer.transform.rotation = Quaternion.Euler(cameraRot.x - 90f, playerRot.y, 0f);

            // other settings
            jammer.name = "OnHandProps";
            jammer.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

            isHoldingJammer = true;

            // TODO: jammer inventory decrease 1
        }

        // hold portal
        if (!isHoldingPortal)
        {
            GameObject portal = Instantiate(portalPrefab, mainCamera.transform);

            // position (right-bottom corner of the screen)
            Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * -1f, 0.6f);
            Vector3 portalPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            portal.transform.position = portalPosition;

            // rotation and size
            Vector3 playerRot = this.transform.rotation.eulerAngles;
            Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
            portal.transform.rotation = Quaternion.Euler(-180f - cameraRot.x, playerRot.y - 180f, -180f);
            portal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            // other settings
            portal.name = "OnHandPortal";
            portal.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
            portal.transform.Find("Particles").gameObject.SetActive(false);
            portal.transform.Find("DarkBackground").gameObject.SetActive(false);

            isHoldingPortal = true;

            // TODO: portal inventory decrease 1
        }

        // hold board
        if (!isHoldingBoard)
        {
            GameObject board = Instantiate(woodBoardPrefab, mainCamera.transform);

            // position (right-bottom corner of the screen)
            Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * 0.1f, 0.6f);
            Vector3 boardPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            board.transform.position = boardPosition;

            // rotation and size
            Vector3 playerRot = this.transform.rotation.eulerAngles;
            Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
            board.transform.rotation = Quaternion.Euler(cameraRot.x - 40f, playerRot.y, 0f);
            board.transform.localScale = new Vector3(0.2f, 0.04f, 0.35f);

            // other settings
            board.name = "OnHandProps";
            board.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

            isHoldingBoard = true;

            // TODO: board inventory decrease 1
        }

        // hold almond water
        if (!isHoldingAlmond)
        {
           GameObject almondWater = Instantiate(almondWaterPrefab, mainCamera.transform);

            // position (right-bottom corner of the screen)
            Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * 0.2f, 0.6f);
            Vector3 almondWaterPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            almondWater.transform.position = almondWaterPosition;

            // rotation and size
            Vector3 playerRot = this.transform.rotation.eulerAngles;
            Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
            almondWater.transform.rotation = Quaternion.Euler(0f - cameraRot.x, playerRot.y - 180f, 0f);
            almondWater.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);

            // other settings
            almondWater.name = "OnHandProps";
            almondWater.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

            isHoldingAlmond = true;
        }

        if (!isHoldingRations)
        {
            // hold rations
            GameObject rations = Instantiate(rationsPrefab, mainCamera.transform);

            // position (right-bottom corner of the screen)
            Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * 0.15f, 0.6f);
            Vector3 rationsPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            rations.transform.position = rationsPosition;

            // rotation and size
            Vector3 playerRot = this.transform.rotation.eulerAngles;
            Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
            rations.transform.rotation = Quaternion.Euler(90f - cameraRot.x, playerRot.y - 180f, cameraRot.z - 30f);
            rations.transform.localScale = new Vector3(4f, 4f, 4f);

            // other settings
            rations.name = "OnHandProps";
            rations.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

            isHoldingRations = true;
        }
    }

    private void PlaceProps()
    {
        // place jammer when raycast doesn't hit anything
        if (isHoldingJammer)
        {
            RaycastHit hit;
            Vector3 rayOrigin = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            bool hasHit = Physics.Raycast(rayOrigin, mainCamera.transform.forward, out hit, 80);

            Vector3 temp = transform.position + transform.forward * 1.5f;
            Vector3 jammerPos = new(temp.x, transform.position.y + 0.47f, temp.z);

            Quaternion jammerRot = Quaternion.Euler(-90f, this.transform.rotation.eulerAngles.y, 0f);

            if (hasHit && hit.transform.CompareTag("Barrier"))
            {
                GameObject jammer = Instantiate(jammerWithPartPrefab, jammerPos, jammerRot);
                jammer.tag = "PlacedJammer";
                hit.transform.GetComponent<BarrierController>().AddNewJammer();
                jammer.GetComponent<JammerController>().setTargetBarrier(hit.transform.gameObject);
            }
            else
            {
                GameObject jammer = Instantiate(jammerPrefab, jammerPos, jammerRot);
                jammer.tag = "PlacedJammer";
            }

            GameObject holdedProps = GameObject.Find("OnHandProps");
            Destroy(holdedProps);

            isHoldingJammer = false;
        }

        // place portal
        if (isHoldingPortal)
        {
            Vector3 temp = transform.position + transform.forward * 2f;
            Vector3 portalPos = new(temp.x, transform.position.y - 1.7f, temp.z);

            float yRot = GetClosestBaseAngle(this.transform.rotation.eulerAngles.y);
            Quaternion portalRot = Quaternion.Euler(0f, yRot, 0f);

            GameObject portal = Instantiate(portalPrefab, portalPos, portalRot);
            portal.tag = "PlacedPortal";
            portalCount++;

            if (portalCount % 2 == 1)
            {
                previousPortal = portal;
                previousPortalPos = transform.position;
            }
            else
            {
                // set tp point
                portal.GetComponent<PortalController>().setTeleportPos(previousPortalPos);
                previousPortal.GetComponent<PortalController>().setTeleportPos(transform.position);

                // set pair portal
                portal.GetComponent<PortalController>().setPairPortal(previousPortal);
                previousPortal.GetComponent<PortalController>().setPairPortal(portal);
            }

            GameObject holdedProps = GameObject.Find("OnHandPortal");
            Destroy(holdedProps);

            isHoldingPortal = false;
        }

        if (isHoldingBoard)
        {
            Vector3 temp = transform.position + transform.forward * 3f;
            Vector3 boardPos = new(temp.x, transform.position.y + 0.05f, temp.z);

            Quaternion boardRot = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0f);

            GameObject board = Instantiate(woodBoardPrefab, boardPos, boardRot);
            board.transform.localScale = new Vector3(1, 0.1f, 1.75f);
            board.tag = "PlacedBoard";

            GameObject holdedProps = GameObject.Find("OnHandProps");
            Destroy(holdedProps);

            isHoldingBoard = false;
        }
    }

    private void UseProps()
    {
        // use almond water
        if (isHoldingAlmond)
        {
            Instantiate(healingCanvas, healingCanvas.transform.position, Quaternion.identity);
            StartCoroutine(Heal(4f, 1));
            isHoldingAlmond = false;
        }

        // use rations
        if (isHoldingRations)
        {
            Instantiate(healingCanvas, healingCanvas.transform.position, Quaternion.identity);
            StartCoroutine(Heal(6f, 2));
            isHoldingRations = false;
        }

        // wear shoes
        //GameObject.Find("Player").GetComponent<FirstPersonController>().WearShoes();
    }

    private IEnumerator Heal(float time, int type)
    {
        GameObject.Find("Player").GetComponent<FirstPersonController>().DisableMovement();

        GameObject.Find("HealingPanel").GetComponent<ProgressBar>().StartCountdown(time);
        yield return new WaitForSeconds(time);

        GameObject.Find("Player").GetComponent<FirstPersonController>().Heal(type);
        GameObject.Find("Player").GetComponent<FirstPersonController>().EnableMovement();

        GameObject holdedProps = GameObject.Find("OnHandProps");
        Destroy(holdedProps);

        yield return new WaitForSeconds(1);
        Destroy(GameObject.FindGameObjectWithTag("Heal"));
    }

    private float GetClosestBaseAngle(float angle)
    {
        float[] baseAngles = { 0f, 90f, 180f, 270f, 360f };

        float closestAngle = baseAngles[0];
        float minDifference = 500f;

        foreach (float baseAngle in baseAngles)
        {
            float difference = Mathf.Abs(angle - baseAngle);
            if (difference < minDifference)
            {
                closestAngle = baseAngle;
                minDifference = difference;
            }
        }

        return closestAngle == 360f ? 0f : closestAngle;
    }

    public void Teleport(Vector3 targetPosition)
    {
        controller.enabled = false;
        controller.transform.position = targetPosition;
        controller.enabled = true;
    }
}
