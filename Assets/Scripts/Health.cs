using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 4;
    public const int MaxHealth = 4;

    // TODO: Overheal should go down over time
    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            GameManager.Instance.playerHealth.Heal();
            GameManager.Instance.killCount++;
            Destroy(gameObject);
            Debug.Log(gameObject.name + "Took dammage");
        }
    }

    public void Heal()
    {
        health = (health >= MaxHealth) ? MaxHealth : health + 1;
        Debug.Log("Healing to: " + health);
    }
}
