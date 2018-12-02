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
    [Range(.01f, .1f)]
    public float slowTurn = .1f;
    [Range(.01f, .1f)]
    public float fastTurn = .01f;

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

        spriteRenderer.sprite = (cooldown <= 0) ? spriteSet.ready : spriteSet.attack;
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
        cooldown -= Time.fixedDeltaTime;
        bool fast = GameManager.Instance.playerHealth.health >= Health.MaxHealth;
        float speed = fast ? fastSpeed : slowSpeed;
        float turn = fast ? fastTurn : slowTurn;

        look.Normalize();

        Vector2 delta = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        delta.Normalize();

        delta *= speed * Time.fixedDeltaTime;
        Vector2 position = myRigidbody2D.position + delta;
        myRigidbody2D.MovePosition(position);

        Collider2D collider = Physics2D.OverlapCircle(myTransform.position, radius, layerMask);
        if (collider != null)
        {
            Health health = collider.GetComponent<Health>();
            if (health != null)
            {
                Debug.Log(health.gameObject.name);
                health.TakeDamage();
                Vector2 stabDirection = collider.transform.position - myTransform.position;
                look.x = RoundDirection(stabDirection.x);
                look.y = RoundDirection(stabDirection.y);
                cooldown = stabDuration;
            }
        }

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90;
        myRigidbody2D.MoveRotation(angle);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
