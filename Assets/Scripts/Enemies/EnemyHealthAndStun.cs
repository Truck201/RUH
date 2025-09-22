using UnityEngine;
using System.Collections;

public class EnemyHealthAndStun : MonoBehaviour
{
    public int maxHealth = 2;
    public float stunDuration = 2f; // tiempo mareado
    private int currentHealth;
    private bool isStunned = false;

    private EnemyAI enemyAI;
    private Rigidbody2D rb;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        // if (isStunned) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject); // O animación de muerte
        }
        else
        {
            StartCoroutine(Stun());
        }
    }

    private IEnumerator Stun()
    {
        isStunned = true;
        enemyAI.enabled = false; // Desactiva la IA
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        enemyAI.enabled = true;
        enemyAI.ChangeState(EnemyAI.State.Idle);
    }
}
