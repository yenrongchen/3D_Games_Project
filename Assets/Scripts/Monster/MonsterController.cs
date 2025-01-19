using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterController : MonoBehaviour
{
    private Animator animator;

    private bool isParalyzed = false;
    private float parTime;

    private Vector3 rayStart;
    private Vector3 offsetPlayer = new(0f, 0.5f, 0f);
    private Vector3 offsetMonster;
    private Vector3 playerPositionForRay;
    private Vector3 rayDirection;
    private int ignoreLayer;

    private Vector3 playerPosition;
    private Vector3 currentPosition;
    private Vector3 moveDirection;
    private float distance;

    private bool playerWalking;
    private float speed;
    private float detectRange;

    private float initialX;
    private float initialZ;

    private float initY;

    [SerializeField]
    private float baseSpeed = 1f;

    [SerializeField]
    private float monsterEyeHeight = 4f;

    [SerializeField]
    private float initialAttackCDFrame = 35f;

    [SerializeField]
    private float wholeAttackCDFrame = 79f;

    private float cd;
    private float initAtkCD;
    private float wholeAtkCD;

    [SerializeField]
    private int typeID = 1;

    [SerializeField]
    private float attackRange = 1.2f;

    [SerializeField]
    private float baseDetectRange = 5f;

    [SerializeField]
    private float paralyzedTime = 2f;

    private bool freezing;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("state", 0);

        initAtkCD = initialAttackCDFrame / 30f;
        wholeAtkCD = wholeAttackCDFrame / 30f;
        cd = initAtkCD;

        offsetMonster = new Vector3(0f, monsterEyeHeight - 1.9f, 0f);

        parTime = paralyzedTime;

        initialX = this.transform.rotation.eulerAngles.x;
        initialZ = this.transform.rotation.eulerAngles.z;

        initY = this.transform.position.y;

        ignoreLayer = ~LayerMask.GetMask("Monster");
    }

    // Update is called once per frame
    void Update()
    {
        // fall into the hole then die
        if (initY - transform.position.y > 5f)
        {
            Destroy(gameObject);
        }

        freezing = GameObject.Find("Player").GetComponent<FirstPersonController>().getFreezing();
        if (freezing)
        {
            animator.SetInteger("state", 0);
            return;
        }

        if (isParalyzed)
        {
            animator.SetInteger("state", 3);
            if (parTime > 0)
            {
                parTime -= Time.deltaTime;
            }
            else
            {
                isParalyzed = false;
                parTime = paralyzedTime;
            }
        }
        else
        {
            playerWalking = GameObject.Find("Player").GetComponent<FirstPersonController>().getIsWalking();
            if (playerWalking)
            {
                speed = baseSpeed;
                detectRange = baseDetectRange;
            }
            else
            {
                speed = baseSpeed * 1.5f;
                detectRange = baseDetectRange * 1.25f;
            }

            playerPosition = GameObject.Find("Player").GetComponent<FirstPersonController>().getPosition();
            playerPositionForRay = playerPosition + offsetPlayer;

            currentPosition = transform.position;
            rayStart = currentPosition + offsetMonster;
            rayDirection = (playerPositionForRay - rayStart).normalized;

            Ray ray = new(rayStart, rayDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 80, ignoreLayer))
            {
                playerPosition.y = 0f;
                currentPosition.y = 0f;
                distance = Vector3.Distance(playerPosition, currentPosition);
                moveDirection = (playerPosition - currentPosition).normalized;

                if (hit.transform.CompareTag("Player"))
                {
                    // facing player
                    transform.LookAt(playerPositionForRay);

                    if (distance <= attackRange)
                    {
                        // attack player
                        animator.SetInteger("state", 2);
                        if (cd > 0)
                        {
                            cd -= Time.deltaTime;
                        }
                        else
                        {
                            Attack(typeID);
                            cd = wholeAtkCD;
                        }
                    }
                    else
                    {
                        // chasing
                        animator.SetInteger("state", 1);
                        this.transform.position += new Vector3(moveDirection.x * speed * Time.deltaTime, 0f, moveDirection.z * speed * Time.deltaTime);
                        cd = initAtkCD;
                    }
                }
                else if (distance <= detectRange)
                {
                    // close to player => approaching slowly
                    transform.LookAt(playerPositionForRay);
                    animator.SetInteger("state", 1);
                    this.transform.position += new Vector3(moveDirection.x * speed / 2 * Time.deltaTime, 0f, moveDirection.z * speed / 2 * Time.deltaTime);
                    cd = initAtkCD;
                }
                else
                {
                    // keep idle
                    animator.SetInteger("state", 0);
                    cd = initAtkCD;
                }
            }
            else
            {
                // keep idle
                animator.SetInteger("state", 0);
                cd = initAtkCD;
            }
        }

        if (this.transform.rotation.eulerAngles.x != initialX)
        {
            Vector3 rotation = this.transform.rotation.eulerAngles;
            rotation.x = initialX;
            this.transform.rotation = Quaternion.Euler(rotation);
        }

        if (this.transform.rotation.eulerAngles.z != initialZ)
        {
            Vector3 rotation = this.transform.rotation.eulerAngles;
            rotation.z = initialZ;
            this.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    private void Attack(int type)
    {
        if (type == 3)
        {
            StartCoroutine(NeedleAttackWithInterval());
        }
        else if (type == 2)
        {
            StartCoroutine(SmilerAttackWithInterval());
        }
        else
        {
            GameObject.Find("Player").GetComponent<FirstPersonController>().Hurt();
        }
    }

    IEnumerator NeedleAttackWithInterval()
    {
        GameObject.Find("Player").GetComponent<FirstPersonController>().Hurt();
        yield return new WaitForSeconds(1f);
        GameObject.Find("Player").GetComponent<FirstPersonController>().Hurt();
        yield return new WaitForSeconds(1f);
        GameObject.Find("Player").GetComponent<FirstPersonController>().Hurt();
    }

    IEnumerator SmilerAttackWithInterval()
    {
        GameObject.Find("Player").GetComponent<FirstPersonController>().Hurt();
        yield return new WaitForSeconds(0.8f);
        GameObject.Find("Player").GetComponent<FirstPersonController>().Hurt();
    }

    public void Paralyzed()
    {
        isParalyzed = true;
    }
}
