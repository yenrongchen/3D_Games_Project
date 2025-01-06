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
                spawnLv1();
                break;

            case 2:
                spawnLv2();
                break;

            case 21:
                spawnLv2e();
                break;

            case 3:
                spawnLv3();
                break;

            case 31:
                spawnLv3e();
                break;

            default:
                spawnLv1();
                break;
        }
    }

    private void spawnLv1()
    {
        float y = 1.9f;

        float minX = -13f;
        float maxX = -8f;
        float minZ = -26f;
        float maxZ = -20f;

        Vector3 opt1 = new(-25f, y, -19.5f);
        Vector3 opt2 = new(-20f, y, -33f);
        Vector3 opt3 = new(-8f, y, -10.5f);

        SpawnWithinRange(minX, maxX, minZ, maxZ, y, bacteriaPrefab);

        Vector3[] spawnPointList = { opt1, opt2, opt3 };
        SpawnWithChoice(spawnPointList, bacteriaPrefab, spawnPointList.Length);
    }

    private void spawnLv2()
    {
        float y = -0.5f;

        float minX_1 = -4f;
        float maxX_1 = 1f;
        float minZ_1 = -8.6f;
        float maxZ_1 = -5f;

        float minX_2 = -1.5f;
        float maxX_2 = 5f;
        float minZ_2 = 13.3f;
        float maxZ_2 = 18.5f;

        Vector3 opt1 = new(4f, y, -18f);
        Vector3 opt2 = new(-20f, y, -5f);

        GameObject[] monsterList = { bacteriaPrefab, smilerPrefab };
        SpawnWithinRange(minX_1, maxX_1, minZ_1, maxZ_1, y, monsterList, monsterList.Length);
        SpawnWithinRange(minX_2, maxX_2, minZ_2, maxZ_2, y, monsterList, monsterList.Length);

        Vector3[] spawnPointList = { opt1, opt2 };
        SpawnWithChoice(spawnPointList, monsterList, spawnPointList.Length, monsterList.Length);
    }

    private void spawnLv2e()
    {
        float y = 0.5f;

        float minX = -11f;
        float maxX = -8f;
        float minZ = -19.2f;
        float maxZ = -16f;

        Vector3 opt1 = new(-8.4f, y, -25.9f);
        Vector3 opt2 = new(-26f, y, -10f);
        Vector3 opt3 = new(0.5f, y, -7f);

        GameObject[] monsterList = { bacteriaPrefab, smilerPrefab };
        SpawnWithinRange(minX, maxX, minZ, maxZ, y, monsterList, monsterList.Length);

        Vector3[] spawnPointList = { opt1, opt2, opt3 };
        SpawnWithChoice(spawnPointList, monsterList, spawnPointList.Length, monsterList.Length);
    }

    private void spawnLv3()
    {

    }

    private void spawnLv3e()
    {

    }

    private void SpawnWithinRange(float minX, float maxX, float minZ, float maxZ, float y, GameObject monster)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        Instantiate(monster, new Vector3(x, y, z), Quaternion.identity);
    }

    private void SpawnWithinRange(float minX, float maxX, float minZ, float maxZ, float y, GameObject[] monsters, int monNum)
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);

        int mon = Random.Range(0, monNum);

        Instantiate(monsters[mon], new Vector3(x, y, z), Quaternion.identity);
    }

    private void SpawnWithChoice(Vector3[] spawnpoints, GameObject monster, int spNum)
    {
        int choice = Random.Range(0, spNum);
        Instantiate(monster, spawnpoints[choice], Quaternion.identity);
    }

    private void SpawnWithChoice(Vector3[] spawnpoints, GameObject[] monsters, int spNum, int monNum)
    {
        int sp = Random.Range(0, spNum);
        int mon = Random.Range(0, monNum);
        Instantiate(monsters[mon], spawnpoints[sp], Quaternion.identity);
    }
}
