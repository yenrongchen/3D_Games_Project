using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    public float rotateAngle = 60f;

    private float timer = 4f;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0f, 0f, rotateAngle * Time.deltaTime));

        if (timer > 2f)
        {
            this.transform.position += new Vector3(0f, 0.05f * Time.deltaTime, 0f);
        }
        else
        {
            this.transform.position -= new Vector3(0f, 0.05f * Time.deltaTime, 0f);
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 4f;
        }
    }
}
