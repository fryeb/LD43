using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GuardController : MonoBehaviour
{
    [Range(1f, 15f)]
    public float speed = 1f;
    [Range(.01f, .1f)]
    public float turn = .1f;

    private Rigidbody2D myRigidbody2D;

    private static readonly Vector2[] directions =
    {
        new Vector2( 0,           1),
        new Vector2( 0.7071068f,  0.7071068f),
        new Vector2( 1,           0),
        new Vector2( 0.7071068f, -0.7071068f),
        new Vector2( 0,          -1),
        new Vector2(-0.7071068f, -0.7071068f),
        new Vector2(-1,           0),
        new Vector2(-0.7071068f,  0.7071068f)
    };

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        int layerMask = (1 << gameObject.layer);
        layerMask = ~layerMask;

        Vector2 playerPosition = (Vector2)GameManager.Instance.playerTransform.position;
        Vector2 thisPosition = (Vector2)transform.position;
       
        float walkableDistance = speed*Time.fixedDeltaTime;
        float distance = Mathf.Infinity;
        Vector2 direction = Vector2.zero;
        Vector2 thisToPlayerDirection = (playerPosition - thisPosition);
        Vector2 lookDirection = Vector2.zero;
        foreach (Vector2 shootCandidate in directions)
        {
            float parallelComponent = Vector3.Dot(shootCandidate, thisToPlayerDirection);
            if (parallelComponent <= 0) continue;

            // Walk direction is the component of thisToPlayerDirection that is perpendicular to shootCandidate
            Vector2 walkCandidate = thisToPlayerDirection - parallelComponent * shootCandidate;

            // Reject candidate if it is further away
            float candidateDistance = walkCandidate.magnitude;
            if (candidateDistance >= distance) continue;

            direction = walkCandidate.normalized;
            lookDirection = shootCandidate;
            distance = candidateDistance;
        }

        myRigidbody2D.MovePosition(thisPosition + direction * (distance > walkableDistance ? walkableDistance : 0));

        RaycastHit2D hit = Physics2D.Raycast(thisPosition, lookDirection, Mathf.Infinity, layerMask);
        if (hit.collider && hit.collider.gameObject.tag == "Player")
            Debug.Log("Shooting the player");

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90;
        if (Mathf.Abs(myRigidbody2D.rotation - angle) > 180)
            myRigidbody2D.MoveRotation(Mathf.Lerp(myRigidbody2D.rotation, angle - 360f, Time.fixedDeltaTime / turn));
        else
            myRigidbody2D.MoveRotation(Mathf.Lerp(myRigidbody2D.rotation, angle, Time.fixedDeltaTime / turn));
    }
}
