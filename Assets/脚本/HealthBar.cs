using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Text healthText;
    private Image healthBar;
    private Player player;
    private int maxHealth;
    private int currentHealth;

    private void Start()
    {
        healthBar = GetComponent<Image>();
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            maxHealth = Player.maxHealth;
            currentHealth = player.currentHealth;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            currentHealth = player.currentHealth;
            maxHealth = Player.maxHealth;
            healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
            healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }
    }
}