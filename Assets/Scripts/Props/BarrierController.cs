using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    private int jammerCount = 0;

    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public Material originalMaterial;
    public Material maskedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void AddNewJammer()
    {
        jammerCount++;
        meshRenderer.material = maskedMaterial;
        meshCollider.isTrigger = true;
    }

    public void RemoveJammer()
    {
        if (jammerCount > 0)
        {
            jammerCount--;

            if (jammerCount == 0)
            {
                meshRenderer.material = originalMaterial;
                meshCollider.isTrigger = false;
            }
        }
    }
}
