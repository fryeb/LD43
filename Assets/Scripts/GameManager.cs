using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float overhealBoost = 1f;
    public float healAmount = 1f;
    public float damageAmount = 1f;
    public int killCount = 0;

    public Health playerHealth;

    private static GameManager instance;
    public static GameManager Instance {  get { return instance; } }

    void Start()
    {
        if (instance != null)
            Debug.LogError("Multiple GameManagers");

        instance = this;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<Health>();
    }
}
