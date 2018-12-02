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
    public int playerHealth = 2;
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
            Debug.LogError("Multiple GameManagers");

        Instance = this;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.GetComponent<Transform>();
    }
}
