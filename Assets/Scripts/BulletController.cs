using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletController : MonoBehaviour {

    public float speed = 1f;

	void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            GameManager.Instance.playerHealth--;
        else
        {
            GuardController guardController = collision.gameObject.GetComponent<GuardController>();
            if (guardController)
                guardController.Respawn();
            else
                Destroy(gameObject);
        }
    }
}
