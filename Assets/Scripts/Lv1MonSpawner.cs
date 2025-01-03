using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv1MonSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bacteriaPrefab;

    private static float y = 1.9f;

    private float minX = -13f;
    private float maxX = -8f;
    private float minZ = -26f;
    private float maxZ = -20f;

    private Vector3 opt1 = new(-25f, y, -19.5f);
    private Vector3 opt2 = new(-20f, y, -33f);
    private Vector3 opt3 = new(-8f, y, -10.5f);

    // Start is called before the first frame update
    void Start()
    {
        SpawnWithinRange(minX, maxX, minZ, maxZ, y);

        List<Vector3> spawnPointList = new() { opt1, opt2, opt3 };
        SpawnWithChoice(spawnPointList);
    }

    private void SpawnWithinRange(float minX, float maxX, float minZ, float maxZ, float y)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        Instantiate(bacteriaPrefab, new Vector3(x, y, z), Quaternion.identity);
    }

    private void SpawnWithChoice(List<Vector3> spawnpoints)
    {
        int choice = Random.Range(0, 3);
        Instantiate(bacteriaPrefab, spawnpoints[choice], Quaternion.identity);
    }
}
