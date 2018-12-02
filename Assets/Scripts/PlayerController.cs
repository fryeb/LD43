using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public SpriteSet spriteSet;

    [Range(.5f, 15f)]
    public float slowSpeed = 1f;
    [Range(.5f, 15f)]
    public float fastSpeed = 5f;

    public float radius = 1f;
    public float stabDuration = 1f;

    private float cooldown = 0f;
    private Rigidbody2D myRigidbody2D;
    private Transform myTransform;
    private SpriteRenderer spriteRenderer;

    private Vector2 look = Vector2.up;
    private int layerMask;

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance.playerHealth <= 0)
            spriteRenderer.sprite = spriteSet.dead;
        else if (cooldown <= 0)
            spriteRenderer.sprite = spriteSet.ready;
        else
            spriteRenderer.sprite = spriteSet.attack;
    }


    float RoundDirection(float x)
    {
        float sign = Mathf.Sign(x);
        if (Mathf.Abs(sign - x) < Mathf.Abs(x))
            return sign;
        else
            return 0;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.playerHealth <= 0)
            return;

        cooldown -= Time.fixedDeltaTime;
        bool fast = GameManager.Instance.playerHealth >= 4;
        float speed = fast ? fastSpeed : slowSpeed;

        look.Normalize();

        Vector2 delta = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (delta.magnitude > .01f) look = delta;

        delta.Normalize();
        delta *= speed * Time.fixedDeltaTime;
        Vector2 position = myRigidbody2D.position + delta;
        myRigidbody2D.MovePosition(position);

        Collider2D collider = Physics2D.OverlapCircle(myTransform.position, radius, layerMask);
        if (collider != null)
        {
            GuardController guard = collider.GetComponent<GuardController>();
            if (guard != null)
            {
                guard.Die(collider.transform.position - myTransform.position);
                Vector2 stabDirection = collider.transform.position - myTransform.position;
                look.x = RoundDirection(stabDirection.x);
                look.y = RoundDirection(stabDirection.y);
                cooldown = stabDuration;
            }
        }

        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg - 90;
        myRigidbody2D.MoveRotation(angle);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
