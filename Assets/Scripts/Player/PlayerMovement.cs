using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem; // Nuevo sistema de inputs

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInputs playerInputs;
    private SpriteRenderer spriteRenderer;

    [Header("Velocidades")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    public Vector2 moveInput;

    public float x, y;

    public float runInput;

    private NavMeshAgent agent;
    private Animator animator;
    private bool isRunning;

    public bool isOnStairs = false;
    [SerializeField] private SceneChanger currentStair;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInputs = new PlayerInputs();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false; // Necesario para 2D
        agent.speed = walkSpeed;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GamePauseManager.Instance.IsPaused) 
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
                agent.velocity = Vector3.zero;
                if (agent.isOnNavMesh)
                    agent.ResetPath();
            }
                return;
        } 
        moveInput = playerInputs.Gameplay.Move.ReadValue<Vector2>();
        runInput = playerInputs.Gameplay.Run.ReadValue<float>();
        isRunning = runInput > 0f;

        agent.speed = isRunning ? runSpeed : walkSpeed;

        // Si hay input, mover
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
            Vector3 targetPos = transform.position + moveDir * 0.5f; // pequeño desplazamiento
            agent.SetDestination(targetPos);
        }
        else
        {
            // Sin input → velocidad en 0 y detener agente
            agent.velocity = Vector3.zero;
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }

        // Animaciones
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsRunning", isRunning);
        }

        if (moveInput.x != 0f)
        {
            moveInput.x = (moveInput.x < 0) ? -1 : 1;
            spriteRenderer.transform.localScale = new Vector3(moveInput.x, 1f, 0f) ;
        }

        //if (isOnStairs) // && playerInputs.Gameplay.Inventory.WasPressedThisFrame()
        //{
        //    currentStair?.ChangeScene();
        //    Debug.Log("Collider !!");
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SceneChanger sceneChanger = other.GetComponent<SceneChanger>();
        if (sceneChanger != null)
        {
            sceneChanger.ChangeScene();
            Debug.Log("Entró al trigger de cambio de escena");
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Stairs"))
    //    {
    //        isOnStairs = true;
    //        currentStair = other.GetComponent<SceneChanger>();
    //        Debug.Log("Entró a las stairs");
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Stairs"))
    //    {
    //        isOnStairs = false;
    //        currentStair = null;
    //        Debug.Log("Salió de las stairs");
    //    }
    //}

    private void OnEnable() => playerInputs.Enable();
    private void OnDisable() => playerInputs.Disable();

    public float getSpriteDirection()
    {
        return moveInput.x;
    }

    public float FlipSprite(int dir)
    {
        return transform.localScale.x * dir;
    }
}
