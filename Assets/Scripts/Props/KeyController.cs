using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public int ID = 1;
    private float rotateAngle = 60f;
    private float speed = 0.08f;
    private float timer = 4f;

    // Update is called once per frame
    void Update()
    {
        if (ID == 1)
        {
            this.transform.Rotate(new Vector3(0f, 0f, rotateAngle * Time.deltaTime));
        }
        else
        {
            this.transform.Rotate(new Vector3(0f, rotateAngle * Time.deltaTime, 0f));
        }
        
        if (timer > 2f)
        {
            this.transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        else
        {
            this.transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 4f;
        }
    }
}
