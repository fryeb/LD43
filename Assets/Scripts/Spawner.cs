using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float boundingRadius = 1f;
    public LayerMask layerMask;
    public Transform[] prefabs;

    private Transform m_Transform;

    int spawnCount = 0;

    void Start()
    {
        m_Transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (spawnCount > GameManager.Instance.killCount)
            return;

        Collider2D hitCollider = Physics2D.OverlapCircle(m_Transform.position, boundingRadius, layerMask);
        if (hitCollider != null)
            return;

        spawnCount++;

        int index = Random.Range(0, prefabs.Length);
        Instantiate(prefabs[index], transform.position, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, boundingRadius);
    }
}
