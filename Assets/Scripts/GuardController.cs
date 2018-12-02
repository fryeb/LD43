using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class GuardController : MonoBehaviour
{
    public SpriteSet spriteSet;
    bool dead = false;

    public Transform bulletPrefab;
    public Vector2 spawnPoint = Vector3.zero;
    [Range(1f, 15f)]
    public float speed = 1f;
    [Range(.01f, 2f)]
    public float walkAwayDistance;
    [Range(.01f, .1f)]
    public float turn = .1f;
    [Range(.1f, 10)]
    public float rateOfFire = 1;
    [Range(0f, 1f)]
    public float warning = 0.5f;

    private float cooldown = 0f;

    private Rigidbody2D myRigidbody2D;
    private Transform myTransform;
    private SpriteRenderer myRenderer;
    private int layerMask;

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
        myRenderer = GetComponent<SpriteRenderer>();
        layerMask = (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Wall"));
    }

    void Update()
    {
        if (dead)
            myRenderer.sprite = spriteSet.dead;
        else if (cooldown > (1 / rateOfFire) * warning)
            myRenderer.sprite = spriteSet.ready;
        else
            myRenderer.sprite = spriteSet.attack;
            
    }

    void FixedUpdate()
    {
        if (dead)
            return;

        cooldown -= Time.fixedDeltaTime;
        Vector2 playerPosition = GameManager.Instance.playerTransform.position;
        Vector2 thisPosition = myTransform.position;
       
        Vector2 thisToPlayerDirection = (playerPosition - thisPosition);

        float walkableDistance = speed*Time.fixedDeltaTime;
        float distance = Mathf.Infinity;
        Vector2 direction = Vector2.zero;
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

        float distanceToPlayer = thisToPlayerDirection.magnitude;
        if (distanceToPlayer <= walkAwayDistance)
            direction = -lookDirection;

        RaycastHit2D lineOfSight = Physics2D.Raycast(thisPosition, thisToPlayerDirection, Mathf.Infinity, layerMask); 
        myRigidbody2D.MovePosition(thisPosition + direction * Mathf.Min(distance, walkableDistance));
        if (lineOfSight.collider == null || lineOfSight.collider.gameObject.tag != "Player")
        {
            lookDirection = direction;
            Debug.Log("cant see player");
        }

        if (distance < walkableDistance) Shoot();

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90;
        myRigidbody2D.MoveRotation(angle);
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

    // Called when player stabs this guard
    public void Die(Vector3 direction)
    {
        if (dead) return; // Can't die if already dead
        myRenderer.sortingLayerName = "Dead";
        myTransform.Rotate(new Vector3(0, 0, 1), 90f);
        GetComponent<Collider2D>().enabled = false;
        GameManager.Instance.playerHealth = Mathf.Min(GameManager.Instance.playerHealth + 1, 4);
        dead = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.TransformPoint(spawnPoint), .05f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, walkAwayDistance);
    }
}
