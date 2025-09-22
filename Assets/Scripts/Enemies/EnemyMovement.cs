using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    private Transform player;
    [SerializeField] private float distance;

    public Vector3 startPoint;
    private Animator animator;

    private SpriteRenderer spriteRenderer;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        startPoint = transform.position;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (GamePauseManager.Instance.IsPaused) return;
        distance = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("Distance", distance);
    }

    //public void Flip(Vector3 objective)
    //{
    //    if (transform.position.x < objective.x) 
    //    {
    //        spriteRenderer.flipX = true; 
    //    } else 
    //    { 
    //        spriteRenderer.flipX = false; 
    //    }
    //}
}
