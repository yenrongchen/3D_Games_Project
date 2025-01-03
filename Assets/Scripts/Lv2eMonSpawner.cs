using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv2eMonSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bacteriaPrefab;

    [SerializeField]
    private GameObject SmilerPrefab;

    private static float y = 0.5f;

    private float minX = -11f;
    private float maxX = -8f;
    private float minZ = -19.2f;
    private float maxZ = -16f;

    private Vector3 opt1 = new(-8.4f, y, -25.9f);
    private Vector3 opt2 = new(-26f, y, -10f);
    private Vector3 opt3 = new(0.5f, y, -7f);

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> monsterList = new() { bacteriaPrefab, SmilerPrefab };
        SpawnWithinRange(minX, maxX, minZ, maxZ, y, monsterList);

        List<Vector3> spawnPointList = new() { opt1, opt2, opt3 };
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
        int sp = Random.Range(0, 3);
        int mon = Random.Range(0, 2);
        Instantiate(monsters[mon], spawnpoints[sp], Quaternion.identity);
    }
}
