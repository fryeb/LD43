using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Range(.5f, 15f)]
    public float slowSpeed = 1f;
    [Range(.5f, 15f)]
    public float fastSpeed = 5f;
    [Range(.01f, .1f)]
    public float slowTurn = .1f;
    [Range(.01f, .1f)]
    public float fastTurn = .01f;

    private Rigidbody2D myRigidbody2D;

    private Vector2 look = Vector2.up;

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        bool fast = GameManager.Instance.playerHealth.health >= Health.MaxHealth;
        float speed = fast ? fastSpeed : slowSpeed;
        float turn = fast ? fastTurn : slowTurn;

        look.Normalize();

        Vector2 delta = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) delta.y++;
        if (Input.GetKey(KeyCode.A)) delta.x--;
        if (Input.GetKey(KeyCode.S)) delta.y--;
        if (Input.GetKey(KeyCode.D)) delta.x++;

        delta *= speed * Time.fixedDeltaTime;
        Vector2 position = myRigidbody2D.position + delta;
        myRigidbody2D.MovePosition(position);

        bool stabbing = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            look.x -= 2;
            stabbing = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            look.x += 2;
            stabbing = true;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            look.y += 2;
            stabbing = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            look.y -= 2;
            stabbing = true;
        }

        if (!stabbing) look = delta;
        else look.Normalize();

        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 90;
        if (Mathf.Abs(myRigidbody2D.rotation - angle) > 180)
            myRigidbody2D.MoveRotation(Mathf.Lerp(myRigidbody2D.rotation, angle - 360f, Time.fixedDeltaTime / turn));
        else
            myRigidbody2D.MoveRotation(Mathf.Lerp(myRigidbody2D.rotation, angle, Time.fixedDeltaTime / turn));
    }
}
