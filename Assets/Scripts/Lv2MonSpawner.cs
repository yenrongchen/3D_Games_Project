using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv2MonSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bacteriaPrefab;

    [SerializeField]
    private GameObject SmilerPrefab;

    private static float y = -0.5f;

    private float minX_1 = -4f;
    private float maxX_1 = 1f;
    private float minZ_1 = -8.6f;
    private float maxZ_1 = -5f;

    private float minX_2 = -1.5f;
    private float maxX_2 = 5f;
    private float minZ_2 = 13.3f;
    private float maxZ_2 = 18.5f;

    private Vector3 opt1 = new(4f, y, -18f);
    private Vector3 opt2 = new(-20f, y, -5f);

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> monsterList = new() { bacteriaPrefab, SmilerPrefab };
        SpawnWithinRange(minX_1, maxX_1, minZ_1, maxZ_1, y, monsterList);
        SpawnWithinRange(minX_2, maxX_2, minZ_2, maxZ_2, y, monsterList);

        List<Vector3> spawnPointList = new() { opt1, opt2 };
        SpawnWithChoice(spawnPointList, monsterList);
    }

    private void SpawnWithinRange(float minX, float maxX, float minZ, float maxZ, float y, List<GameObject> monsters)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        int mon = Random.Range(0, 2);

        Instantiate(monsters[mon], new Vector3(x, y, z), Quaternion.identity);
    }

    private void SpawnWithChoice(List<Vector3> spawnpoints, List<GameObject> monsters)
    {
        int sp = Random.Range(0, 2);
        int mon = Random.Range(0, 2);
        Instantiate(monsters[mon], spawnpoints[sp], Quaternion.identity);
    }
}
