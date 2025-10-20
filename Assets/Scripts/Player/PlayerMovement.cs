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

    [Header("Stamina")]
    public float maxStamina = 3.5f;       // Cuánto dura la corrida (en segundos aprox)
    public float staminaRecoveryRate = 2f; // cuánto recupera por segundo al caminar/detenerse
    public float staminaDrainRate = 1f; // cuánto gasta por segundo al correr
    private float currentStamina;
    private bool exhausted; // 🔹 si se vacía, no puede correr hasta que recupere

    public Vector2 moveInput;
    public float runInput;

    private NavMeshAgent agent;
    private Animator animator;
    public bool isRunning;

    public bool isOnStairs = false;
    [SerializeField] private SceneChanger currentStair;
    [SerializeField] WhirlwindWeapon weapon;

    private bool spriteUp;
    private bool spriteDown;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInputs = new PlayerInputs();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false; // Necesario para 2D
        agent.speed = walkSpeed;

        animator = GetComponent<Animator>();

        currentStamina = maxStamina;
    }

    private void Start()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.SetPlayer(this.transform);
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

        bool wantsToRun = runInput > 0f && currentStamina > 0f && !exhausted;

        if (wantsToRun)
        {
            isRunning = true;
            agent.speed = runSpeed;
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                exhausted = true; // ⚠️ se vació → entra en estado exhausto
            }
        }
        else
        {
            isRunning = false;
            agent.speed = walkSpeed;

            // Recuperar stamina cuando no corres
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;

                if (currentStamina >= maxStamina * 0.5f)
                {
                    exhausted = false; // ✅ cuando recupera al menos la mitad, ya puede volver a correr
                }

                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;
            }
        }

        //agent.speed = isRunning ? runSpeed : walkSpeed;

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


        if (moveInput.y > 0) 
        {
            spriteUp = true;
            spriteDown = false;

            weapon.frontVaccum = true;
            weapon.backVaccum = false;
        }
        else if (moveInput.y < 0)
        {
            spriteUp = false;
            spriteDown = true;

            weapon.frontVaccum = false;
            weapon.backVaccum = true;
        } else
        {
            spriteUp = false;
            spriteDown = false;

            weapon.frontVaccum = false;
            weapon.backVaccum = false;
        }

        // Animaciones
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("Up", spriteUp);
            animator.SetBool("Down", spriteDown);
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

    public float GetStaminaNormalized() => currentStamina / maxStamina;

    private void OnEnable() => playerInputs.Enable();
    private void OnDisable() => playerInputs.Disable();

    public float getSpriteDirection() => moveInput.x;

    public float FlipSprite(int dir) => transform.localScale.x * dir;
}
