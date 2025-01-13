using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using TMPro;

public class PlayerActionController : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    [Header("Props")]
    public GameObject portalPrefab;
    public GameObject jammerPrefab;
    public GameObject crosshairPrefab;
    public GameObject woodBoardPrefab;
    public GameObject almondWaterPrefab;
    public GameObject rationsPrefab;

    [Header("Jammer settings")]
    public float jammerAtkCD = 8f;

    // character controller
    private CharacterController controller;

    // props
    private List<string> showOutline = new() { "Props", "Jammer", "Key1", "Key2", "Key3", "PlacedJammer", "PlacedBoard" };
    private List<string> canPick = new() { "Props", "Jammer", "Key1", "Key2", "Key3" };

    // jammer
    private bool isHoldingJammer = false;
    private GameObject crosshair;
    private List<string> jammerTargetTags = new() { "Barrier", "Enemy" };
    private float atkcd = 0f;

    // portal
    private bool isHoldingPortal = false;
    private GameObject previousPortal;
    private Vector3 previousPortalPos;
    private int portalCount = 0;

    // wood board
    private bool isHoldingBoard = false;

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
                if (Input.GetKey(KeyCode.Mouse0) && hit.transform.CompareTag("Enemy") && atkcd <= 0)
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

        // hold props
        if (Input.GetKeyDown(KeyCode.E))  // KEY BIND NEED FIX
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
        if (hit.transform.CompareTag("Jammer"))
        {
            // TODO: jammer inventory +1
        }

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

        // pick keys
        if (hit.transform.CompareTag("Key1") || hit.transform.CompareTag("Key2") || hit.transform.CompareTag("Key3"))
        {
            GameObject circlebase = GameObject.Find("CircleBase");
            Destroy(circlebase);

            // TODO: key inventory +1
        }

        // rations and almond water


        // TODO: gems inventory +1


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
        }
    }

    private void HoldProps()
    {
        //hasPropsOnHand = true;

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
            jammer.transform.rotation = Quaternion.Euler(-90f, this.transform.rotation.eulerAngles.y, 0f);

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
            portal.transform.rotation = Quaternion.Euler(-195f, this.transform.rotation.eulerAngles.y - 165f, -180f);
            portal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            // other settings
            portal.name = "OnHandPortal";
            portal.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
            portal.transform.Find("Particles").gameObject.SetActive(false);
            portal.transform.Find("DarkBackground").gameObject.SetActive(false);

            isHoldingPortal = true;

            // TODO: jammer inventory decrease 1
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

            GameObject jammer = Instantiate(jammerPrefab, jammerPos, jammerRot);
            jammer.tag = "PlacedJammer";

            if (hasHit && hit.transform.CompareTag("Barrier"))
            {
                hit.transform.GetComponent<BarrierController>().AddNewJammer();
                jammer.GetComponent<JammerController>().setTargetBarrier(hit.transform.gameObject);
            }

            GameObject holdedProps = GameObject.Find("OnHandProps");
            Destroy(holdedProps);

            isHoldingJammer = false;

            // TODO: jammer inventory -1
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

            // TODO: portal inventory -1
        }

        if (isHoldingBoard)
        {
            // board.size = (1, 0.1, 1.75)
            //GameObject board = Instantiate(woodBoardPrefab, pos, rot);
            //board.tag = "PlacedBoard";

            //GameObject holdedProps = GameObject.Find("OnHandProps");
            //Destroy(holdedProps);

            // TODO: board inventory -1
        }

        //hasPropsOnHand = false;
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
