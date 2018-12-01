using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAttackController : MonoBehaviour
{
    public SpriteSet spriteSet;
    public Vector2 originOffset = Vector2.zero;
    public float range = 1f;

    private bool attacking = false;

    private SpriteRenderer spriteRenderer;
    private LayerMask layerMask;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        layerMask = ~(1 << gameObject.layer);
    }

    void Update()
    {
        attacking = Input.GetMouseButton(0);
        spriteRenderer.sprite = attacking ? spriteSet.attack : spriteSet.ready;

        if (attacking)
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.TransformPoint(originOffset), range * (Vector2)transform.up, range, layerMask);
            if (hit.distance <= range && hit.collider)
            {
                Debug.Log("hit something");
                Health health = hit.collider.GetComponent<Health>();
                if (health)
                    health.TakeDamage();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = attacking ? Color.red : Color.green;
        Vector2 origin = transform.TransformPoint(originOffset);
        Gizmos.DrawLine(origin, origin + range * (Vector2)transform.up);
    }

}
