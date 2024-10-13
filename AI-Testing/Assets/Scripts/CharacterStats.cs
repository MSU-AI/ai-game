using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int attack = 10;
    public int defense = 5;
    public float moveSpeed = 5f;

    private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - defense, 0);
        currentHealth -= damageTaken;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("GetHitTrigger");
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void Die()
    {
        animator.SetTrigger("DeathTrigger");
        // Additional death logic here (e.g., disable character controls, etc.)
        Debug.Log(gameObject.name + " has died.");
    }
}