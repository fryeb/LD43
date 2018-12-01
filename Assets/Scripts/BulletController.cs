using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletController : MonoBehaviour {

    float speed = 1f;

	void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Health health = collision.collider.GetComponent<Health>();
        if (health)
            health.TakeDamage();
        Destroy(gameObject);
    }
}
