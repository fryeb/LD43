﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class HUDController : MonoBehaviour
{
    private Animator animator;

    public Sprite[] hearts;

    public Image heartImage;

    private int previousHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        previousHealth = GameManager.Instance.playerHealth.health;
    }

    void Update()
    {
        int health = GameManager.Instance.playerHealth.health;
        if (health > previousHealth)
            animator.SetTrigger("heal");
        else if (health < previousHealth)
            animator.SetTrigger("damage");

        previousHealth = health;
    }

    void UpdateHeart()
    {
        int health = GameManager.Instance.playerHealth.health;
        if (health > hearts.Length) health = hearts.Length;
        if (health <= 0)
            heartImage.sprite = null;
        else
            heartImage.sprite = hearts[health - 1];
    }
}