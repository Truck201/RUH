using UnityEngine;

public class StoneProjectile : MonoBehaviour
{
    public int damage = 1;
    public float explosionDelay = 2f;
    public GameObject explosionEffect;
    public bool isProyectible = false;

    [Header("Physics")]
    public float deceleration = 2f; // 🔹 desaceleración por segundo
    public float stopThreshold = 0.05f; // 🔹 velocidad mínima para considerarse quieto

    private Rigidbody2D rb;
    private Vector2 startPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startPosition = transform.position;

        if (!isProyectible) return;
        Invoke(nameof(Explode), explosionDelay);
    }

    private void Update()
    {
        if (!isProyectible) return;

        float speed = rb.linearVelocity.magnitude;
        if (speed > stopThreshold)
        {
            // Aplicar fricción proporcional a la velocidad
            float decelAmount = deceleration * Time.fixedDeltaTime;
            float newSpeed = Mathf.Max(speed - decelAmount, 0f);
            rb.linearVelocity = rb.linearVelocity.normalized * newSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Explota si recorrió cierta distancia (opcional)
        if (Vector2.Distance(startPosition, transform.position) > 5f)
        {
            Debug.Log("--> Active Explode");
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isProyectible) return;
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealthAndStun enemy = collision.GetComponent<EnemyHealthAndStun>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // Destruir la piedra al impactar
        }

        //if (collision.gameObject.CompareTag("Pedido"))
        //{
        //    string ingrediente = this.tag; // el proyectil tiene tag "Zanahoria" / "Papa" / "Cebolla"
        //    DeliverManager manager = Object.FindFirstObjectByType<DeliverManager>();
        //    //manager.RegistrarImpactoIngrediente(ingrediente);
        //    Destroy(gameObject);
        //}
    }
}
