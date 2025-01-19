using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
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
    public GameObject woodBoardPrefab;
    public GameObject almondWaterPrefab;
    public GameObject rationsPrefab;

    [Header("Jammer settings")]
    public float jammerAtkCD = 10f;

    [Header("UI")]
    public GameObject healingCanvas;

    // props
    private bool holdingProps = false;
    private List<string> showOutline = new() { "Props", "Jammer", "Key", "Gem", "PlacedJammer", "PlacedBoard" };
    private List<string> canPick = new() { "Props", "Jammer", "Portal" };
    private List<string> canRetrieve = new() { "PlacedPortal", "PlacedBoard", "PlacedJammer", "JammerWithPart" };

    // jammer
    private bool isHoldingJammer = false;
    private float atkcd = 0f;
    private List<string> jammerTargetTags = new() { "Barrier", "Monster" };
    private List<string> canDirectHold = new() { "Jammer", "PlacedJammer", "JammerWithPart" };

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
    private bool isHealing = false;

    // pick and retrieve distance
    private Vector3 modPlayerPos;
    private Vector3 modTargetPos;
    private float distance;

    // backpack
    private bool isOpeningBackpack = false;
    private GameObject backpack;

    // UI
    private GameObject crosshair;
    private GameObject skill;


    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Start()
    {
        crosshair = GameObject.Find("Crosshair");
        crosshair.SetActive(false);

        skill = GameObject.Find("SkillIcon");
        skill.SetActive(false);

        backpack = GameObject.Find("Backpack");
        backpack.SetActive(false);
    }

    void Update()
    {
        CancelAllOutlines();

        // raycast along with view
        RaycastHit hit;
        Vector3 rayOrigin = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(rayOrigin, mainCamera.transform.forward, out hit, 80))
        {
            // show props outlines
            ShowOutline(hit);

            modPlayerPos = new(transform.position.x, 0f, transform.position.z);
            modTargetPos = new(hit.transform.position.x, 0f, hit.transform.position.z);
            distance = Vector3.Distance(modPlayerPos, modTargetPos);

            if (distance < 2.4f && !holdingProps)
            {
                // pick props
                if (Input.GetKey(KeyCode.F))
                {
                    if (canPick.Contains(hit.transform.tag))
                    {
                        PickProps(hit);
                    }
                    if (hit.transform.CompareTag("Key"))
                    {
                        GetComponent<FirstPersonController>().Freeze();
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        Flowchart.BroadcastFungusMessage("Key");
                    }
                    if (hit.transform.CompareTag("Gem"))
                    {
                        GetComponent<FirstPersonController>().Freeze();
                        Flowchart.BroadcastFungusMessage("Gem");
                    }
                }

                // retrieve props
                if (Input.GetKey(KeyCode.R) && canRetrieve.Contains(hit.transform.tag))
                {
                    RetrieveProps(hit);
                }
            }

            // hold jammer directly
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDirectHold.Contains(hit.transform.tag) && !holdingProps && !isOpeningBackpack)
            {
                if (hit.transform.CompareTag("JammerWithPart"))
                {
                    // update barrier status
                    GameObject jammer = hit.transform.gameObject;
                    GameObject barrier = jammer.GetComponentInChildren<JammerController>().getTargetBarrier();
                    barrier.GetComponent<BarrierController>().RemoveJammer();
                }

                Destroy(hit.transform.gameObject);

                HoldJammer();
            }

            // jammer-specific action
            if (isHoldingJammer)
            {
                crosshair.SetActive(true);
                skill.SetActive(true);

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
                    skill.GetComponent<SkillCoolDown>().Countdown(atkcd);
                }

                // paralyze monster
                if (Input.GetKeyDown(KeyCode.Mouse0) && hit.transform.CompareTag("Monster"))
                {
                    if (atkcd <= 0 && !isOpeningBackpack)
                    {
                        GameObject monster = hit.transform.gameObject;
                        monster.GetComponent<MonsterController>().Paralyzed();
                        atkcd = jammerAtkCD;
                    }
                }
            }
            else
            {
                crosshair.SetActive(false);
                skill.SetActive(false);
            }
        }

        // place props
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            PlaceProps();
        }

        // use props
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseProps();
        }

        // open or close backpack
        bool teleporting = GetComponentInChildren<PlayerInteraction>().CheckTeleporting();
        bool paused = GameObject.Find("GameManager").GetComponent<GameManager>().GetPaused();
        if (Input.GetKeyDown(KeyCode.Q) && !teleporting && !paused)
        {
            if (isOpeningBackpack)
            {
                CloseBackpack();
            }
            else
            {
                OpenBackpack();
            }
        }

        // put back holding props
        if (Input.GetKeyDown(KeyCode.C) && holdingProps && !isHealing)
        {
            PutBackProps();
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

        GameObject[] allJammerWP = GameObject.FindGameObjectsWithTag("JammerWithPart");
        foreach (GameObject jammer in allJammerWP)
        {
            var outline = jammer.GetComponentInChildren<Outline>();
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

        if (hit.transform.CompareTag("Portal") || hit.transform.CompareTag("PlacedPortal") || hit.transform.CompareTag("JammerWithPart"))
        {
            var outline = hit.transform.gameObject.GetComponentInChildren<Outline>();
            outline.enabled = true;
        }
    }

    private void PickProps(RaycastHit hit)
    {
        // pick portal
        if (hit.transform.CompareTag("Portal"))
        {
            GameObject pairPortal = hit.transform.gameObject.GetComponent<PortalController>().getPairPortal();
            if (pairPortal != null)
            {
                pairPortal.GetComponent<PortalController>().DisableTeleport();
            }
        }

        // pick props and store into backpack
        hit.transform.GetComponent<ItemPickup>().Pickup();
    }

    private void PickKey()
    {
        GameObject key = GameObject.FindGameObjectWithTag("Key");
        key.GetComponent<ItemPickup>().Pickup();

        Destroy(key);
        GameObject circlebase = GameObject.Find("CircleBase");
        if (circlebase != null) Destroy(circlebase);

        GetComponent<FirstPersonController>().Unfreeze();
        GetComponentInChildren<MazeDialog>().Finish();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PickGem()
    {
        GameObject gem = GameObject.FindGameObjectWithTag("Gem");
        gem.GetComponent<ItemPickup>().Pickup();

        Destroy(gem);
        GetComponent<FirstPersonController>().Unfreeze();
    }

    private void RetrieveProps(RaycastHit hit)
    {
        // retrieve jammer
        if (hit.transform.CompareTag("JammerWithPart"))
        {
            // update barrier status
            GameObject jammer = hit.transform.gameObject;
            GameObject barrier = jammer.GetComponentInChildren<JammerController>().getTargetBarrier();
            if (barrier != null)
            {
                barrier.GetComponent<BarrierController>().RemoveJammer();
            }
        }

        // retrieve portal
        if (hit.transform.CompareTag("PlacedPortal"))
        {
            GameObject portal = hit.transform.gameObject;
            portalCount--;

            GameObject pairPortal = portal.GetComponent<PortalController>().getPairPortal();
            if (pairPortal != null)
            {
                pairPortal.GetComponent<PortalController>().DisableTeleport();
            }
        }

        // store into backpack
        hit.transform.GetComponent<ItemPickup>().Pickup();
    }

    public void HoldJammer()
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
        jammer.transform.gameObject.GetComponent<MeshCollider>().enabled = false;

        isHoldingJammer = true;
        holdingProps = true;
    }

    public void HoldPortal()
    {
        GameObject portal = Instantiate(portalPrefab, mainCamera.transform);

        Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * -1f, 0.6f);
        Vector3 portalPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        portal.transform.position = portalPosition;

        Vector3 playerRot = this.transform.rotation.eulerAngles;
        Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
        portal.transform.rotation = Quaternion.Euler(-180f - cameraRot.x, playerRot.y - 180f, -180f);
        portal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        portal.name = "OnHandPortal";
        portal.transform.gameObject.GetComponent<BoxCollider>().enabled = false;
        portal.transform.Find("Particles").gameObject.SetActive(false);
        portal.transform.Find("DarkBackground").gameObject.SetActive(false);

        isHoldingPortal = true;
        holdingProps = true;
    }

    public void HoldBoard()
    {
        GameObject board = Instantiate(woodBoardPrefab, mainCamera.transform);

        Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * 0.1f, 0.6f);
        Vector3 boardPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        board.transform.position = boardPosition;

        Vector3 playerRot = this.transform.rotation.eulerAngles;
        Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
        board.transform.rotation = Quaternion.Euler(cameraRot.x - 40f, playerRot.y, 0f);
        board.transform.localScale = new Vector3(0.2f, 0.04f, 0.35f);

        board.name = "OnHandProps";
        board.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

        isHoldingBoard = true;
        holdingProps = true;
    }

    public void HoldAlmond()
    {
        GameObject almondWater = Instantiate(almondWaterPrefab, mainCamera.transform);

        Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * 0.2f, 0.6f);
        Vector3 almondWaterPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        almondWater.transform.position = almondWaterPosition;

        Vector3 playerRot = this.transform.rotation.eulerAngles;
        Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
        almondWater.transform.rotation = Quaternion.Euler(0f - cameraRot.x, playerRot.y - 180f, 0f);
        almondWater.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);

        almondWater.name = "OnHandProps";
        almondWater.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

        isHoldingAlmond = true;
        holdingProps = true;
    }

    public void HoldRations()
    {
        GameObject rations = Instantiate(rationsPrefab, mainCamera.transform);

        Vector3 screenPosition = new(Screen.width * 0.85f, Screen.height * 0.15f, 0.6f);
        Vector3 rationsPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        rations.transform.position = rationsPosition;

        Vector3 playerRot = this.transform.rotation.eulerAngles;
        Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
        rations.transform.rotation = Quaternion.Euler(90f - cameraRot.x, playerRot.y - 180f, cameraRot.z - 30f);
        rations.transform.localScale = new Vector3(4f, 4f, 4f);

        rations.name = "OnHandProps";
        rations.transform.gameObject.GetComponent<BoxCollider>().enabled = false;

        isHoldingRations = true;
        holdingProps = true;
    }

    private void PlaceProps()
    {
        // place jammer
        if (isHoldingJammer)
        {
            RaycastHit hit;
            Vector3 rayOrigin = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            bool hasHit = Physics.Raycast(rayOrigin, mainCamera.transform.forward, out hit, 80);

            Vector3 temp = transform.position + transform.forward * 1.2f;
            Vector3 jammerPos = new(temp.x, transform.position.y + 0.47f, temp.z);

            if (hasHit && hit.transform.CompareTag("Barrier"))
            {
                Quaternion jammerRot = Quaternion.Euler(0f, this.transform.rotation.eulerAngles.y, 0f);

                GameObject jammer = Instantiate(jammerWithPartPrefab, jammerPos, jammerRot);
                jammer.tag = "JammerWithPart";

                hit.transform.GetComponent<BarrierController>().AddNewJammer();
                jammer.GetComponentInChildren<JammerController>().setTargetBarrier(hit.transform.gameObject);
            }
            else
            {
                Quaternion jammerRot = Quaternion.Euler(-90f, this.transform.rotation.eulerAngles.y, 0f);

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
            Vector3 temp = transform.position + transform.forward * 1.2f;
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

        // place board
        if (isHoldingBoard)
        {
            Vector3 temp = transform.position + transform.forward * 2f;
            Vector3 boardPos = new(temp.x, transform.position.y + 0.05f, temp.z);

            float yRot = GetClosestBaseAngle(this.transform.rotation.eulerAngles.y);
            Quaternion boardRot = Quaternion.Euler(0f, yRot, 0f);

            GameObject board = Instantiate(woodBoardPrefab, boardPos, boardRot);
            board.transform.localScale = new Vector3(1, 0.1f, 1.75f);
            board.tag = "PlacedBoard";

            GameObject holdedProps = GameObject.Find("OnHandProps");
            Destroy(holdedProps);

            isHoldingBoard = false;
        }

        holdingProps = false;
    }

    private void UseProps()
    {
        // use almond water
        if (isHoldingAlmond)
        {
            if (GameObject.FindGameObjectWithTag("CountDownBar") != null)
            {
                return;
            }
            Instantiate(healingCanvas);
            StartCoroutine(Heal(4f, 1));
        }

        // use rations
        if (isHoldingRations)
        {
            if (GameObject.FindGameObjectWithTag("CountDownBar") != null)
            {
                return;
            }
            Instantiate(healingCanvas);
            StartCoroutine(Heal(6f, 2));
        }
    }

    private IEnumerator Heal(float time, int type)
    {
        FirstPersonController player = GameObject.Find("Player").GetComponent<FirstPersonController>();

        player.DisableMovement();
        isHealing = true;

        GameObject.Find("HealingPanel").GetComponent<ProgressBar>().StartCounterCountdown(time);
        yield return new WaitForSeconds(time);

        player.Heal(type);
        player.EnableMovement();

        GameObject holdedProps = GameObject.Find("OnHandProps");
        Destroy(holdedProps);

        isHoldingAlmond = false;
        holdingProps = false;
        isHealing = false;

        yield return new WaitForSeconds(0.8f);
        Destroy(GameObject.FindGameObjectWithTag("CountDownBar"));
    }

    private void PutBackProps()
    {
        GameObject props = GameObject.Find("OnHandProps") ?? GameObject.Find("OnHandPortal");
        Item item = props.GetComponent<ItemController>().item;

        InventoryManager.Instance.Add(item);

        switch (item.id)
        {
            case 1:
                isHoldingJammer = false;
                break;
            case 2:
                isHoldingPortal = false;
                break;
            case 3:
                isHoldingBoard = false;
                break;
            case 4:
                isHoldingAlmond = false;
                break;
            case 5:
                isHoldingRations = false;
                break;
        }

        Destroy(props);
        holdingProps = false;
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

    public void OpenBackpack()
    {
        backpack.SetActive(true);
        isOpeningBackpack = true;

        // show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GetComponent<FirstPersonController>().Freeze();
    }

    public void CloseBackpack()
    {
        backpack.SetActive(false);
        isOpeningBackpack = false;

        // hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GetComponent<FirstPersonController>().Unfreeze();
    }

    public bool GetHolding()
    {
        return holdingProps;
    }
}
