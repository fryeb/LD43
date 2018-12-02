using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class HUDController : MonoBehaviour
{
    private Animator animator;

    public Sprite[] hearts;

    public Image heartImage;
    public Text scoreText;

    private int previousHealth;

    void Start()
    {
        animator = GetComponent<Animator>();
        previousHealth = GameManager.Instance.playerHealth;
    }

    void Update()
    {
        int health = GameManager.Instance.playerHealth;
        if (health <= 0 && previousHealth > 0)
            animator.SetTrigger("die");
        else if (health > previousHealth)
            animator.SetTrigger("heal");
        else if (health < previousHealth && health > 0)
            animator.SetTrigger("damage");

        previousHealth = health;

        scoreText.text = GameManager.Instance.killCount.ToString();
    }

    void UpdateHeart()
    {
        int health = GameManager.Instance.playerHealth;
        if (health >= hearts.Length)
            health = hearts.Length - 1;

        heartImage.sprite = hearts[health];
        heartImage.color = Color.white;
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}