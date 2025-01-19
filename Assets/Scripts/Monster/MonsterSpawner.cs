using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    // specify which level to spawn
    public int level;

    [SerializeField]
    private GameObject bacteriaPrefab;

    [SerializeField]
    private GameObject smilerPrefab;

    [SerializeField]
    private GameObject needlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        switch (level)
        {
            case 1:
                SpawnLv1();
                break;

            case 2:
                SpawnLv2();
                break;

            case 21:
                SpawnLv2e();
                break;

            case 3:
                SpawnLv3();
                break;

            case 31:
                SpawnLv3e();
                break;
        }
    }

    private void SpawnLv1()
    {
        float y = 1.9f;

        float minX = -13f;
        float maxX = -8f;
        float minZ = -26f;
        float maxZ = -20f;

        Vector3 opt1 = new(-20f, y, -13.7f);
        Vector3 opt2 = new(-8f, y, -10.5f);
        Vector3 opt3 = new(-5f, y, -31.8f);

        SpawnWithinRange(minX, maxX, minZ, maxZ, y, bacteriaPrefab);

        Vector3[] spawnPointList = { opt1, opt2, opt3 };
        SpawnWithChoice(spawnPointList, bacteriaPrefab);
    }

    private void SpawnLv2()
    {
        float y = -0.55f;

        float minX_1 = 4f;
        float maxX_1 = 7.5f;
        float minZ_1 = -18.5f;
        float maxZ_1 = -16f;

        float minX_2 = -1.5f;
        float maxX_2 = 5f;
        float minZ_2 = 13.3f;
        float maxZ_2 = 18.5f;

        Vector3 opt1 = new(-2.2f, y, -7.3f);
        Vector3 opt2 = new(-20f, y, -5f);
        Vector3 opt3 = new(-26f, y, 13f);

        // ensure at least one smiler
        SpawnWithinRange(minX_2, maxX_2, minZ_2, maxZ_2, y, smilerPrefab);

        GameObject[] monsterList = { bacteriaPrefab, smilerPrefab };
        SpawnWithinRange(minX_1, maxX_1, minZ_1, maxZ_1, y, monsterList);
        
        Vector3[] spawnPointList = { opt1, opt2, opt3 };
        SpawnWithChoice(spawnPointList, monsterList);
    }

    private void SpawnLv2e()
    {
        float y = 0.5f;

        float minX = -11f;
        float maxX = -8f;
        float minZ = -19.2f;
        float maxZ = -16f;

        Vector3 opt1 = new(-8.4f, y, -25.7f);
        Vector3 opt2 = new(-26f, y, -10f);
        Vector3 opt3 = new(-1.6f, y, -10f);

        // ensure at least one smiler
        SpawnWithinRange(minX, maxX, minZ, maxZ, y, smilerPrefab);

        Vector3[] spawnPointList = { opt1, opt2, opt3 };
        GameObject[] monsterList = { bacteriaPrefab, smilerPrefab };
        SpawnWithChoice(spawnPointList, monsterList);
    }

    private void SpawnLv3()
    {
        float y = -0.5f;

        float minX_1 = -18f;
        float maxX_1 = -13.5f;
        float minZ_1 = -22.5f;
        float maxZ_1 = -18f;

        float minX_2 = -22.5f;
        float maxX_2 = -18f;
        float minZ_2 = -22.5f;
        float maxZ_2 = -18f;

        float minX_3 = -22.5f;
        float maxX_3 = -13.5f;
        float minZ_3 = -18f;
        float maxZ_3 = -13.5f;

        SpawnWithinRange(minX_1, maxX_1, minZ_1, maxZ_1, y, bacteriaPrefab);
        SpawnWithinRange(minX_2, maxX_2, minZ_2, maxZ_2, y, smilerPrefab);
        SpawnWithinRange(minX_3, maxX_3, minZ_3, maxZ_3, y, needlePrefab);

        float minX_4 = -56f;
        float maxX_4 = -48f;
        float minZ_4 = -46f;
        float maxZ_4 = -42f;

        float minX_5 = -56f;
        float maxX_5 = -48f;
        float minZ_5 = -42f;
        float maxZ_5 = -38f;

        GameObject[] monsterList = { bacteriaPrefab, smilerPrefab, needlePrefab };
        SpawnWithinRange(minX_4, maxX_4, minZ_4, maxZ_4, y, monsterList);
        SpawnWithinRange(minX_5, maxX_5, minZ_5, maxZ_5, y, needlePrefab);

        float minX_6 = -42.5f;
        float maxX_6 = -33.5f;
        float minZ_6 = -42f;
        float maxZ_6 = -38f;

        SpawnWithinRange(minX_6, maxX_6, minZ_6, maxZ_6, y, needlePrefab);

        Vector3 opt1 = new(-38f, y, -44f);
        Vector3 opt2 = new(-19f, y, -46.5f);

        Vector3[] spawnPointList_1 = { opt1, opt2 };
        SpawnWithChoice(spawnPointList_1, monsterList);

        float minX_7 = -36f;
        float maxX_7 = -31f;
        float minZ_7 = -24.5f;
        float maxZ_7 = -17.5f;

        SpawnWithinRange(minX_7, maxX_7, minZ_7, maxZ_7, y, smilerPrefab);

        Vector3 opt3 = new(-28.5f, y, -21f);
        Vector3 opt4 = new(-36f, y, -10f);

        Vector3[] spawnPointList_2 = { opt3, opt4 };
        SpawnWithChoice(spawnPointList_2, monsterList);
    }

    private void SpawnLv3e()
    {
        float y = -0.5f;

        float minX_1 = -22f;
        float maxX_1 = -18f;
        float minZ_1 = -32f;
        float maxZ_1 = -28f;

        float minX_2 = -22f;
        float maxX_2 = -18f;
        float minZ_2 = -28f;
        float maxZ_2 = -24f;

        float minX_3 = -24f;
        float maxX_3 = -22f;
        float minZ_3 = -32f;
        float maxZ_3 = -24f;

        SpawnWithinRange(minX_1, maxX_1, minZ_1, maxZ_1, y, bacteriaPrefab);
        SpawnWithinRange(minX_2, maxX_2, minZ_2, maxZ_2, y, smilerPrefab);
        SpawnWithinRange(minX_3, maxX_3, minZ_3, maxZ_3, y, needlePrefab);

        float minX_4 = -6.6f;
        float maxX_4 = -1.6f;
        float minZ_4 = -36f;
        float maxZ_4 = -32f;

        float minX_5 = -6.6f;
        float maxX_5 = -1.6f;
        float minZ_5 = -32f;
        float maxZ_5 = -28f;

        GameObject[] monsterList = { bacteriaPrefab, smilerPrefab, needlePrefab };
        SpawnWithinRange(minX_4, maxX_4, minZ_4, maxZ_4, y, monsterList);
        SpawnWithinRange(minX_5, maxX_5, minZ_5, maxZ_5, y, needlePrefab);

        float minX_6 = -22f;
        float maxX_6 = -18f;
        float minZ_6 = -16f;
        float maxZ_6 = -11.5f;

        SpawnWithinRange(minX_6, maxX_6, minZ_6, maxZ_6, y, smilerPrefab);

        Vector3 opt1 = new(-25f, y, -11f);
        Vector3 opt2 = new(-38.5f, y, -4f);

        Vector3[] spawnPointList = { opt1, opt2 };
        SpawnWithChoice(spawnPointList, monsterList);
    }

    private void SpawnWithinRange(float minX, float maxX, float minZ, float maxZ, float y, GameObject monster)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        Instantiate(monster, new Vector3(x, y, z), Quaternion.identity);
    }

    private void SpawnWithinRange(float minX, float maxX, float minZ, float maxZ, float y, GameObject[] monsters)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        int mon = Random.Range(0, monsters.Length);

        Instantiate(monsters[mon], new Vector3(x, y, z), Quaternion.identity);
    }

    private void SpawnWithChoice(Vector3[] spawnpoints, GameObject monster)
    {
        int sp = Random.Range(0, spawnpoints.Length);
        Instantiate(monster, spawnpoints[sp], Quaternion.identity);
    }

    private void SpawnWithChoice(Vector3[] spawnpoints, GameObject[] monsters)
    {
        int sp = Random.Range(0, spawnpoints.Length);
        int mon = Random.Range(0, monsters.Length);
        Instantiate(monsters[mon], spawnpoints[sp], Quaternion.identity);
    }
}
