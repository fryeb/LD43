using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] prefabs;
    int spawnCount = 0;

    void Update()
    {
        if (spawnCount > GameManager.Instance.killCount)
            return;

        spawnCount++;

        int index = Random.Range(0, prefabs.Length);
        Instantiate(prefabs[index], transform.position, Quaternion.identity);
    }
}
