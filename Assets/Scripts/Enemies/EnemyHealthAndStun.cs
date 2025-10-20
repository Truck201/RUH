using UnityEngine;
using System.Collections;

public class EnemyHealthAndStun : MonoBehaviour
{
    public int maxHealth = 2;
    public float stunDuration = 2f; // tiempo mareado
    private int currentHealth;

    private EnemyMovement enemyMovement;
    private Animator animator;

    // 🧩 Prefabs para los drops
    [Header("Drop Prefabs")]
    public GameObject zanahoriaPrefab;
    public GameObject papaPrefab;
    public GameObject cebollaPrefab;

    // guardamos el último proyectil que lo golpeó
    private string lastHitSource;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyMovement = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount, string sourceName = "")
    {
        currentHealth -= amount;
        lastHitSource = sourceName;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Stun();
        }
    }

    private void Stun()
    {
        if (enemyMovement)
        {
            enemyMovement.stunned = true;
            animator.SetBool("Stunned", true);
            StartCoroutine(StunCoroutine());
        }
    }

    private IEnumerator StunCoroutine()
    {
        yield return new WaitForSeconds(stunDuration);
        enemyMovement.stunned = false;
        animator.SetBool("Stunned", false);
    }
    private void Die()
    {
        animator.SetTrigger("Die");

        // 🧩 Determinar qué objeto soltar
        GameObject dropPrefab = null;

        if (!string.IsNullOrEmpty(lastHitSource))
        {
            if (lastHitSource.Contains("Zanahoria"))
                dropPrefab = zanahoriaPrefab;
            else if (lastHitSource.Contains("Papa"))
                dropPrefab = papaPrefab;
            else if (lastHitSource.Contains("Cebolla"))
                dropPrefab = cebollaPrefab;
        }

        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 1f);
    }
}
