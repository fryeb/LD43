using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float overhealBoost = 1f;
    public float healAmount = 1f;
    public float damageAmount = 1f;
    public int killCount = 0;

    public Transform playerTransform;
    public Health playerHealth;

    private static GameManager instance;
    public static GameManager Instance {  get { return instance; } }

    void Awake()
    {
        if (instance != null)
            Debug.LogError("Multiple GameManagers");

        instance = this;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<Health>();
    }
}
