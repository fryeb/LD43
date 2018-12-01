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
    [Range(.1f, 10)]
    public float rateOfFire = 1;

    private float cooldown = 0f;

    private Rigidbody2D myRigidbody2D;
    private Transform myTransform;

    public Transform bulletPrefab;
    public Vector2 spawnPoint = Vector3.zero;

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
        myTransform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        cooldown -= Time.fixedDeltaTime;

        int layerMask = (1 << gameObject.layer);
        layerMask = ~layerMask;

        Vector2 playerPosition = (Vector2)GameManager.Instance.playerTransform.position;
        Vector2 thisPosition = (Vector2)myTransform.position;
       
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

            // Reject candidate if it is further away than current best
            float candidateDistance = walkCandidate.magnitude;
            if (candidateDistance >= distance) continue;

            // Reject candidate if destination is unreachable
            RaycastHit2D hit = Physics2D.Raycast(thisPosition, walkCandidate, candidateDistance, layerMask);
            if (hit.collider != null)
                continue;

            // Reject candidate if on ariving at the destination the shot at the player is blocked
            Vector3 destination = thisPosition + walkCandidate;
            hit = Physics2D.Raycast(destination, shootCandidate, Mathf.Infinity, layerMask);
            if (hit.collider == null || hit.collider.gameObject.tag != "Player")
                continue;

            direction = walkCandidate.normalized;
            lookDirection = shootCandidate;
            distance = candidateDistance;
        }

        myRigidbody2D.MovePosition(thisPosition + direction * Mathf.Min(distance, walkableDistance));

        if (distance < walkableDistance) Shoot();

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90;
        if (Mathf.Abs(myRigidbody2D.rotation - angle) > 180)
            myRigidbody2D.MoveRotation(Mathf.Lerp(myRigidbody2D.rotation, angle - 360f, Time.fixedDeltaTime / turn));
        else
            myRigidbody2D.MoveRotation(Mathf.Lerp(myRigidbody2D.rotation, angle, Time.fixedDeltaTime / turn));

    }

    void Shoot()
    {
        // We only fire when cooled down
        if (cooldown > 0)
            return;

        Vector3 transformedSpawnPoint = myTransform.TransformPoint(spawnPoint);
        Instantiate(bulletPrefab, transformedSpawnPoint, myTransform.rotation, null);

        cooldown = 1 / rateOfFire;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.TransformPoint(spawnPoint), .05f);
    }
}
