using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] prefabs;
    public float minCooldown = 0f;
    public float maxCooldown = 1f;
    private float cooldown = 0f;


    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown > 0 || GameManager.Instance.playerHealth <= 0)
            return;

        Debug.Assert(minCooldown < maxCooldown);
        cooldown = Random.Range(minCooldown, maxCooldown);

        int index = Random.Range(0, prefabs.Length);
        Instantiate(prefabs[index], transform.position, Quaternion.identity);
    }
}
