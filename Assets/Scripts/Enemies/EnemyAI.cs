using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Walk, Chase }
    public State currentState;

    [Header("Movimiento")]
    public float idleTime = 2f;                 // Tiempo quieto
    public float walkSpeed = 2f;                // Velocidad caminando
    public float chaseSpeed = 4f;               // Velocidad persiguiendo
    public float patrolRange = 5f;              // Radio de patrullaje
    public Transform patrolCenter;              // Centro de patrulla
    public float detectionDistance = 3f;        // Radio de detección
    public LayerMask playerLayer;               // Capa del jugador

    public float stuckThreshold = 0.2f;
    public float stuckTime = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private Vector2 targetPosition;             // Punto hacia donde caminar
    private float currentSpeed;

    private float stuckTimer;
    private Vector2 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        lastPosition = rb.position;
        ChangeState(State.Idle);
    }

    void Update()
    {
        if (GamePauseManager.Instance.IsPaused) return;
        switch (currentState)
        {
            case State.Idle:
                animator.SetFloat("Speed", 0f);
                break;

            case State.Walk:
                animator.SetFloat("Speed", currentSpeed);
                if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
                {
                    ChangeState(State.Idle);
                }
                break;

            case State.Chase:
                animator.SetFloat("Speed", currentSpeed);
                targetPosition = player.position;
                break;
        }

        DetectPlayer();
    }

    void FixedUpdate()
    {
        if (GamePauseManager.Instance.IsPaused) return;
        if (currentState == State.Walk || currentState == State.Chase)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, currentSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            DetectStuck();
        }
    }

    public void ChangeState(State newState)
    {
        StopAllCoroutines();
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                currentSpeed = 0f;
                StartCoroutine(IdleState());
                break;

            case State.Walk:
                currentSpeed = walkSpeed;
                PickRandomPatrolPoint();
                break;

            case State.Chase:
                currentSpeed = chaseSpeed;
                break;
        }
    }

    IEnumerator IdleState()
    {
        yield return new WaitForSeconds(idleTime);
        ChangeState(State.Walk);
    }

    void PickRandomPatrolPoint()
    {
        Vector2 randomPoint = (Vector2)patrolCenter.position + Random.insideUnitCircle * patrolRange;
        targetPosition = randomPoint;
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionDistance, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            ChangeState(State.Chase);
        }
        else if (currentState == State.Chase && Vector2.Distance(transform.position, player.position) > detectionDistance)
        {
            ChangeState(State.Idle);
        }
    }

    void DetectStuck()
    {
        float movedDistance = Vector2.Distance(rb.position, lastPosition);
        float realSpeed = movedDistance / Time.fixedDeltaTime;

        if (realSpeed < stuckThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTime)
            {
                // Se considera atascado
                Debug.Log("Enemigo atascado, eligiendo nuevo punto.");
                ChangeState(State.Idle);
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f; // Reset si se mueve normalmente
        }

        lastPosition = rb.position;
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja el rango de patrulla
        Gizmos.color = Color.green;
        if (patrolCenter != null)
            Gizmos.DrawWireSphere(patrolCenter.position, patrolRange);

        // Dibuja el rango de detección
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}
