﻿using System.Collections;
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
    [Range(1f, 15f)]
    public float shootDistance = 1f;
    [Range(.01f, 2f)]
    public float walkAwayDistance;
    [Range(.01f, .1f)]
    public float turn = .1f;
    [Range(.1f, 10)]
    public float rateOfFire = 1;
    [Range(0f, 1f)]
    public float warning = 0.5f;
    public float fadeOutDuration = 1f;

    private float cooldown = 0f;

    private Rigidbody2D myRigidbody2D;
    private Transform myTransform;
    private SpriteRenderer myRenderer;
    private int layerMask;

    private Vector3 origin;

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
        origin = myTransform.position;
        cooldown = 1 / rateOfFire;
    }

    void Update()
    {
        if (dead)
        {
            myRenderer.sprite = spriteSet.dead;
            Color color = myRenderer.color;
            if (color.a < 0) Destroy(gameObject);
            color.a -= Time.deltaTime / fadeOutDuration;
            myRenderer.color = color;
        }
        else if (cooldown > (1 / rateOfFire) * warning)
            myRenderer.sprite = spriteSet.ready;
        else
            myRenderer.sprite = spriteSet.attack;
            
    }

    void FixedUpdate()
    {
        if (dead || GameManager.Instance.playerHealth <= 0)
            return;

        cooldown -= Time.fixedDeltaTime;
        Vector2 playerPosition = GameManager.Instance.playerTransform.position;
        Vector2 thisPosition = myTransform.position;
       
        Vector2 thisToPlayerDirection = (playerPosition - thisPosition);

        float walkableDistance = speed*Time.fixedDeltaTime;
        float distance = Mathf.Infinity;
        Vector2 direction = Vector2.zero;
        Vector2 shootDirection = Utils.RoundDirection(thisToPlayerDirection);
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

            direction += walkCandidate.normalized;
            lookDirection = shootCandidate;
            distance = candidateDistance;
        }

        myRigidbody2D.MovePosition(thisPosition + direction.normalized * Mathf.Min(distance, walkableDistance));

        // Dont look at the player if we cant see them
        RaycastHit2D lineOfSight = Physics2D.Raycast(thisPosition, thisToPlayerDirection, Mathf.Infinity, layerMask);
        if (lineOfSight.collider == null || lineOfSight.collider.gameObject.tag != "Player")
        {
            lookDirection = direction;
        }
        else if (thisToPlayerDirection.magnitude > shootDistance)
        {
            direction = lookDirection;
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
        GameManager.Instance.killCount++;
        Instantiate(GameManager.Instance.bloodPrefab, myTransform.position, Quaternion.identity);
        dead = true;
    }

    public void Respawn()
    {
        myTransform.position = origin;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.TransformPoint(spawnPoint), .05f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, walkAwayDistance);
    }
}
