using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private Animator animator;

    private Vector3 rayStart;
    private Vector3 playerPosition;
    private Vector3 offset = new(0f, 0.5f, 0f);
    private Vector3 direction;
    private float distance;

    [SerializeField]
    private float initAtkCD = 35f / 30f;

    [SerializeField]
    private float wholeAtkCD = 79f / 30f;

    private float cd;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("state", 0);
        cd = initAtkCD;
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = GameObject.Find("Player").GetComponent<FirstPersonController>().getPosition() + offset;

        rayStart = new Vector3(transform.position.x, 1f, transform.position.z);
        direction = (playerPosition - rayStart).normalized;

        distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - playerPosition.x, 2f) + Mathf.Pow(transform.position.z - playerPosition.z, 2f));
        
        Ray ray = new(rayStart, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.name == "Player" && distance <= 20f)
            {
                // facing player
                transform.LookAt(playerPosition);

                if (distance <= 1.2f)
                {
                    // attack player
                    animator.SetInteger("state", 2);

                    // check attack cd
                    if (cd > 0)
                    {
                        cd -= Time.deltaTime;
                    }
                    else
                    {
                        // attack
                        GameObject.Find("Player").GetComponent<FirstPersonController>().Hurt();
                        cd = wholeAtkCD;
                    }
                }
                else
                {
                    // chasing
                    animator.SetInteger("state", 1);
                    this.transform.position += new Vector3(direction.x * Time.deltaTime, 0f, direction.z * Time.deltaTime);
                    cd = initAtkCD;
                }
            }
            else if (distance <= 5)
            {
                // close to player => approaching slowly
                transform.LookAt(playerPosition);
                animator.SetInteger("state", 1);
                this.transform.position += new Vector3(direction.x * Time.deltaTime / 4, 0f, direction.z * Time.deltaTime / 4);
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
}
