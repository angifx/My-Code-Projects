using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public HealthUI healthUI; 
    private SpriteRenderer spriteRenderer; 
    public static event Action OnPlayerDied; 

    void Start()
    {
        ResetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
       
        GameController.OnReset += ResetHealth; 
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GegnerMovement gegner = collision.GetComponent<GegnerMovement>();
        if (gegner)
        {
            TakeDamage(gegner.damage);
        }

        trap trap = collision.GetComponent<trap>();
        if (trap && trap.damage < 0)
        {
            TakeDamage(trap.damage); 
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);
        SoundEffectMangager.Play("playerHit"); 
        StartCoroutine(FlashRed()); 

        if(currentHealth <= 0)
        {
            OnPlayerDied.Invoke(); 
            SoundEffectMangager.Play("playerDeath"); 
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}
