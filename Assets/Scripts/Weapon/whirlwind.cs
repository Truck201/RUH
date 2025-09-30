using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WhirlwindWeapon : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public float suctionRadius = 3f;
    public Transform suctionPoint; // donde las piedras se �pegan�
    public LayerMask stoneLayer;
    public float suctionForce = 5f;

    [SerializeField] PlayerMovement playerManager;
    [SerializeField] Animator playerAnimator;
    [SerializeField] ToolScroller toolScroller;

    [Header("Attack Settings")]
    public float shootForce = 10f;
    public float projectileLifetime = 2f;

    private PlayerInputs playerInputs;
    private bool isVacuuming = false;

    private void Awake()
    {
        playerInputs = new PlayerInputs();

        playerInputs.Gameplay.Vaccum.performed += ctx => StartVacuum();
        playerInputs.Gameplay.Vaccum.canceled += ctx => StopVacuum();
        playerInputs.Gameplay.Attack.performed += ctx => Attack();
    }
    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    private void Update()
    {
        if (isVacuuming)
        {
            Suction();
        }
    }
    private void StartVacuum()
    {
        isVacuuming = true;
    }
    private void StopVacuum()
    {
        isVacuuming = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(suctionPoint.position, suctionRadius, stoneLayer);
        foreach (var hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    private void Suction()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(suctionPoint.position, suctionRadius, stoneLayer);
        foreach (var hit in hits)
        {
            //Debug.Log($" detection hit -> {hit}");
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb)
            {
                Vector2 dir = ((Vector2)suctionPoint.position - rb.position).normalized;
                rb.linearVelocity = dir * suctionForce;

                // Si está suficientemente cerca, la succiona y destruye
                if (Vector2.Distance(rb.position, suctionPoint.position) < 0.3f)
                {
                    CollectibleStone stoneData = hit.GetComponent<CollectibleStone>();
                    if (stoneData)
                    {
                        if (toolScroller.CanAddAmmo(stoneData.stoneName))
                        {
                            toolScroller.AddAmmo(
                                stoneData.stoneName,
                                stoneData.stoneIcon,
                                stoneData.projectilePrefab,
                                1
                            );
                            Destroy(rb.gameObject);
                        }
                        else
                        {
                            Debug.Log("No hay espacio para más ammo");
                        }
                    }
                    Destroy(rb.gameObject);
                }
            }
        }
    }
    private void Attack()
    {
        AmmoSlot activeAmmo = toolScroller.ammoSlots[toolScroller.activeSlotIndex];
        GameObject prefabToShoot = activeAmmo.projectilePrefab;

        // 🔹 Verificar munición antes de disparar
        if (!toolScroller.UseAmmo())
            return;

        if (prefabToShoot == null)
        {
            Debug.LogWarning("Intentaste disparar pero el prefab es null.");
            return;
        }

        if (playerAnimator) {
            playerAnimator.SetTrigger("Shoot");
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 shootDirection = (mouseWorldPos - transform.position).normalized;

        // 🔹 Corregir dirección del sprite si apunta al otro lado
        if ((shootDirection.x > 0 && playerManager.getSpriteDirection() < 0) ||
            (shootDirection.x < 0 && playerManager.getSpriteDirection() > 0))
        {
            playerManager.FlipSprite(shootDirection.x > 0 ? 1 : -1);
        }

        // 🔹 Instanciar proyectil
        GameObject projectile = Instantiate(prefabToShoot, suctionPoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDirection * shootForce;

        // 🔹 Configurar StoneProjectile
        StoneProjectile sp = projectile.GetComponent<StoneProjectile>();
        sp.explosionDelay = projectileLifetime;
        sp.isProyectible = true;

        Debug.Log("Disparo realizado. Munición restante: " + activeAmmo.count);
    }

    private void OnDrawGizmosSelected()
    {
        if (suctionPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(suctionPoint.position, suctionRadius);
        }
    }
}
