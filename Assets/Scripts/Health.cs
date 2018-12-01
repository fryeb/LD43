using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [System.NonSerialized] public float health = 1f;
    public float maxHealth = 1f;

    void Start()
    {
        health = maxHealth;
    }

    public float GetSpeedBoost()
    { 
        float overheal = Mathf.Clamp01(health - maxHealth);
        return overheal * GameManager.Instance.overhealBoost;
    }

    // TODO: Overheal should go down over time

    public void TakeDamage()
    {
        health -= GameManager.Instance.damageAmount;
        if (health <= 0)
        {
            GameManager.Instance.playerHealth.Heal();
            Destroy(gameObject);
        }

        Debug.Log("hit");
    }

    public void Heal()
    {
        health += GameManager.Instance.healAmount;
        Debug.Log("Healing to: " + health);
    }
}
