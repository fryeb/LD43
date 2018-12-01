using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HeartUIController : MonoBehaviour
{

    public Sprite[] hearts;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        int health = GameManager.Instance.playerHealth.health;
        if (health > hearts.Length) health = hearts.Length;
        if (health <= 0)
            image.sprite = null;
        else
            image.sprite = hearts[health - 1];
    }
}
