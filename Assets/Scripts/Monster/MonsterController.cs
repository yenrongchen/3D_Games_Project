using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private Animator animator;
    private bool isParalyzed = false;
    private float parTime;

    private Vector3 rayStart;
    private Vector3 playerPosition;
    private Vector3 offsetPlayer = new(0f, 0.5f, 0f);
    private Vector3 offsetMonster;
    private Vector3 direction;
    private float distance;

    [SerializeField]
    private float speed = 1f;

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
    private float detectRange = 5f;

    [SerializeField]
    private float paralyzedTime = 2f;

    // to be deleted //
    //LineRenderer lr;

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

        // to be deleted //
        //lr = GetComponent<LineRenderer>();
        //lr.endWidth = 0.05f;
        //lr.startWidth = 0.05f;
        //lr.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
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
            playerPosition = GameObject.Find("Player").GetComponent<FirstPersonController>().getPosition() + offsetPlayer;

            rayStart = transform.position + offsetMonster;
            direction = (playerPosition - rayStart).normalized;

            distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - playerPosition.x, 2f) + Mathf.Pow(transform.position.z - playerPosition.z, 2f));

            Ray ray = new(rayStart, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Player")
                {
                    // facing player
                    transform.LookAt(playerPosition);

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
                        this.transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0f, direction.z * speed * Time.deltaTime);
                        cd = initAtkCD;
                    }
                }
                else if (distance <= detectRange)
                {
                    // close to player => approaching slowly
                    transform.LookAt(playerPosition);
                    animator.SetInteger("state", 1);
                    this.transform.position += new Vector3(direction.x * speed / 2 * Time.deltaTime, 0f, direction.z * speed / 2 * Time.deltaTime);
                    cd = initAtkCD;
                }
                else
                {
                    // keep idle
                    animator.SetInteger("state", 0);
                    cd = initAtkCD;
                }

                // to be deleted //
                //lr.enabled = true;  // if hit object then show the line
                //lr.SetPosition(0, rayStart);  //line start 
                //lr.SetPosition(1, hit.point);       // line end
            }
            else
            {
                // keep idle
                animator.SetInteger("state", 0);
                cd = initAtkCD;
            }
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
