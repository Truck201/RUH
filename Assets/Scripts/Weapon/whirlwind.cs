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
    [SerializeField] CollectibleManager collectibleManager;

    [Header("Attack Settings")]
    public float shootForce = 10f;
    public float projectileLifetime = 2f;

    [SerializeField] Animator playerAnims;

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
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb)
            {
                Vector2 dir = ((Vector2)suctionPoint.position - rb.position).normalized;
                rb.linearVelocity = dir * suctionForce;

                if (Vector2.Distance(rb.position, suctionPoint.position) < 0.3f)
                {
                    Collectible collectibleData = hit.GetComponent<Collectible>();
                    if (collectibleData)
                    {
                        switch (collectibleData.type)
                        {
                            case CollectibleType.Stone:
                                if (collectibleManager.CanAddAmmo(collectibleData.itemName))
                                {
                                    collectibleManager.AddAmmo(
                                        collectibleData.itemName,
                                        collectibleData.itemIcon,
                                        collectibleData.projectilePrefab,
                                        1
                                    );
                                }
                                else
                                {
                                    Debug.Log("No hay espacio para más piedras");
                                }
                                break;

                            case CollectibleType.Veggie:
                                collectibleManager.AddVeggie(
                                    collectibleData.itemName,
                                    collectibleData.itemIcon,
                                    1
                                );
                                break;
                        }
                    }

                    Destroy(rb.gameObject);
                }
            }
        }
    }

    private void Attack()
    {
        AmmoSlot activeAmmo = collectibleManager.ammoSlots[collectibleManager.activeSlotIndex];

        // 🔹 Chequeo de munición ANTES de gastar
        if (activeAmmo == null || activeAmmo.IsEmpty() || activeAmmo.count <= 0)
        {
            Debug.LogWarning("No hay munición para disparar.");
            return;
        }

        GameObject prefabToShoot = activeAmmo.projectilePrefab;
        if (prefabToShoot == null)
        {
            Debug.LogWarning("Intentaste disparar pero el prefab es null.");
            return;
        }

        // 🔹 Dirección de disparo
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 shootDirection = (mouseWorldPos - transform.position).normalized;

        // 🔹 Flip del sprite si cambia de dirección
        if ((shootDirection.x > 0 && playerManager.getSpriteDirection() < 0) ||
            (shootDirection.x < 0 && playerManager.getSpriteDirection() > 0))
        {
            playerManager.FlipSprite(shootDirection.x > 0 ? 1 : -1);
        }

        // 🔹 Instanciar proyectil
        GameObject projectile = Instantiate(prefabToShoot, suctionPoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = shootDirection * shootForce;

        // Config extra
        StoneProjectile sp = projectile.GetComponent<StoneProjectile>();
        sp.explosionDelay = projectileLifetime;
        sp.isProyectible = true;

        // 🔹 Ahora sí gastamos la munición
        collectibleManager.UseAmmo();

        if (playerAnims)
            playerAnims.SetTrigger("Shoot");

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
