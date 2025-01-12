using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerActionController : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;

    [Header("Props")]
    public GameObject portalPrefab;
    public GameObject jammerPrefab;
    public GameObject crosshairPrefab;

    // character controller
    private CharacterController controller;

    // line renderer (for debug)
    //private LineRenderer lr;

    // props
    private List<string> tags = new() { "Props" };

    // jammer
    private GameObject crosshair;
    private bool isHoldingJammer = false;

    // portal
    private List<GameObject> portals = new List<GameObject>();
    private Vector3 previousPortalPos;
    private int portalCount = 0;
    private bool isHoldingPortal = true;  // should be false initially

    void Start()
    {
        controller = GetComponent<CharacterController>();
        crosshair = Instantiate(crosshairPrefab, Vector3.zero, Quaternion.identity);
        //lr = GetComponent<LineRenderer>();
        //lr.endWidth = 0.05f;
        //lr.startWidth = 0.05f;
        //lr.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // clear outlines of all objects
        CancelAllOutlines();

        // show props outlines and jammer's action
        RaycastHit hit;
        Vector3 rayPosition;
        rayPosition = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(rayPosition, mainCamera.transform.forward, out hit, 99))
        {
            //lr.enabled = true;
            //lr.SetPosition(0, rayPos);
            //lr.SetPosition(1, hit.point);
            ShowOutline(hit);
            JammerAction(hit);
            ShowCrosshair(isHoldingJammer, hit);
        }

        // portal's action
        PortalAction();
    }

    private void JammerAction(RaycastHit hit)
    {
        // TODO: take jammer
        if (Input.GetKeyDown(KeyCode.I) && hit.transform.CompareTag("Jammer") && !isHoldingJammer)
        {
            //Destroy(targetJammer);

            // jammer inventory +1
        }

        // hold jammer
        if (Input.GetKeyDown(KeyCode.J) && !isHoldingJammer)
        {
            GameObject jammer = Instantiate(jammerPrefab, mainCamera.transform);

            // position (right-bottom corner of the screen)
            Vector3 screenPosition = new(Screen.width * 0.8f, Screen.height * -0.25f, 0.8f);
            Vector3 jammerPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            jammer.transform.position = jammerPosition;

            // rotation
            jammer.transform.rotation = Quaternion.Euler(-90f, this.transform.rotation.eulerAngles.y, 0f);

            // other settings
            jammer.name = "onHandJammer";
            jammer.tag = "OnHandProps";
            jammer.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

            isHoldingJammer = true;

            // TODO: jammer inventory decrease 1
        }

        // place jammer
        if (isHoldingJammer && Input.GetKeyDown(KeyCode.U))
        {
            if (hit.transform.tag == "Barrier")
            {
                // set barrier to invisible
                hit.transform.GetComponent<MeshRenderer>().enabled = false;
                hit.transform.GetComponent<MeshCollider>().enabled = false;
            }

            Vector3 jammerPos = new(transform.position.x, 0.45f, transform.position.z - 2f);
            Instantiate(jammerPrefab, jammerPos, Quaternion.Euler(-90f, this.transform.rotation.eulerAngles.y, 0f));

            GameObject holdedJammer = GameObject.Find("onHandJammer");
            Destroy(holdedJammer);
            isHoldingJammer = false;
        }

        // retrieve jammer
        if (Input.GetKeyDown(KeyCode.M))
        {
            // TODO: retrieve jammer

            // set barrier to visible
            //hit.transform.GetComponent<MeshRenderer>().enabled = true;
            //hit.transform.GetComponent<MeshCollider>().enabled = true;
        }

        // TODO: paralyze monster
        if (isHoldingJammer && Input.GetKeyDown(KeyCode.Mouse0))
        {

        }
    }

    private void PortalAction()
    {
        // take portal
        if (Input.GetKeyDown(KeyCode.B))
        {
            // TODO: destroy target portal (maybe use raycast?)
            //Destroy(targetPortal);

            GameObject[] portalsInScene = GameObject.FindGameObjectsWithTag("Portal");
            if (portalsInScene.Length % 2 == 1)
            {
                portalsInScene[^1].GetComponent<PortalController>().DisableTeleport();
            }

            // TODO: portal inventory increase 1
        }

        // TODO: hold portal


        // place portal
        if (isHoldingPortal && Input.GetKeyDown(KeyCode.T))
        {
            // TODO: Portal placed position and check angle
            Vector3 portalPos = new(transform.position.x, -1.7f, transform.position.z - 2f);
            GameObject portal = Instantiate(portalPrefab, portalPos, Quaternion.Euler(0, 180, 0));
            portals.Add(portal);
            portalCount++;

            if (portalCount % 2 == 1)
            {
                previousPortalPos = transform.position;
            }
            else
            {
                portal.GetComponent<PortalController>().setTeleportPos(previousPortalPos);
                portals[^2].GetComponent<PortalController>().setTeleportPos(transform.position);
            }

            // TODO: portal inventory decrease 1
        }

        // retrieve portal
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (portalCount > 0)
            {
                Destroy(portals[^1]);
                portals.RemoveAt(portalCount - 1);
                portalCount--;

                if (portalCount % 2 == 1)
                {
                    portals[^1].GetComponent<PortalController>().DisableTeleport();
                }
            }

            // TODO: portal inventory increase 1
        }
    }

    private void ShowOutline(RaycastHit hit)
    {
        if (tags.Contains(hit.transform.tag))
        {
            var outline = hit.transform.gameObject.GetComponent<Outline>();
            outline.enabled = true;
        }
        if (hit.transform.tag == "Portal")
        {
            var outline = hit.transform.gameObject.GetComponentInChildren<Outline>();
            outline.enabled = true;
        }
    }

    private void CancelAllOutlines()
    {
        GameObject[] allProps = GameObject.FindGameObjectsWithTag("Props");
        foreach (GameObject props in allProps)
        {
            var outline = props.GetComponent<Outline>();
            outline.enabled = false;
        }
        GameObject[] allOnHandProps = GameObject.FindGameObjectsWithTag("OnHandProps");
        foreach (GameObject onhandprops in allOnHandProps)
        {
            var outline = onhandprops.GetComponent<Outline>();
            outline.enabled = false;
        }
        GameObject[] allPortals = GameObject.FindGameObjectsWithTag("Portal");
        foreach (GameObject port in allPortals)
        {
            var outline = port.GetComponentInChildren<Outline>();
            outline.enabled = false;
        }
    }

    private void ShowCrosshair(bool holding, RaycastHit hit)
    {
        if (holding)
        {
            // show crosshair
            crosshair.SetActive(true);
            crosshair.transform.position = hit.point;
        }
        else
        {
            crosshair.SetActive(false);
        }
    }

    public void Teleport(Vector3 targetPosition)
    {
        controller.enabled = false;
        controller.transform.position = targetPosition;
        controller.enabled = true;
    }
}
