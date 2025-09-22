using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float idleTime = 2f;       
    public float walkTime = 3f;       
    public float walkSpeed = 2f;     
    public Vector2 walkDirection = Vector2.right; // Direcci�n base
    public bool loop = true;

    private Rigidbody2D rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        StartCoroutine(PatrolLoop());
    }

    private System.Collections.IEnumerator PatrolLoop()
    {
        while (loop)
        {
            // IDLE
            anim?.SetTrigger("Idle");
            rb.linearVelocity = Vector2.zero;
            yield return new WaitForSeconds(idleTime);

            // WALK
            anim?.SetTrigger("Walk");
            rb.linearVelocity = walkDirection.normalized * walkSpeed;
            yield return new WaitForSeconds(walkTime);

            // Vuelve al idle y cambia direcci�n
            walkDirection *= -1; // invierte la direcci�n
        }
    }

    private void OnDrawGizmos()
    {
        // Dibujar la direcci�n de caminata en el editor
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)walkDirection.normalized * 2f);
    }
}
