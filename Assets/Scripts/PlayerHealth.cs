using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayerDied;

    // this line is to allow access to the onother script
    public HealthUI healthUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetHealth();
        // gets the sprite renderer of this object is attached to
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this isto get the Eneny script from whatever the object this script is atteched to, collided with 
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy) // if enemy exists
        {
            TakeDamage(enemy.damage);
        }

        // this is to get the "Trap" script from whatever the object this script is atteched to, collided with
        Trap trap = collision.GetComponent<Trap>();
        if (trap && trap.damage > 0)
        {
            TakeDamage(trap.damage);
        }
    }

    void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthUI.UpdateHearts(currentHealth);
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage)
    {
        StartCoroutine(FlashRed());
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        if (currentHealth <= 0)
        {
            // game over code
            OnPlayerDied.Invoke(); // this is how events are called and used in code
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}
