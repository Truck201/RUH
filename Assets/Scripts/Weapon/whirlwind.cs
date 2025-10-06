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

    [Header("Vaccum Player - Object distance")]
    public float playerObjectVaccumDistance = 0.4f;

    [SerializeField] PlayerMovement playerManager;
    [SerializeField] CollectibleManager collectibleManager;

    [Header("Attack Settings")]
    public float shootForce = 10f;
    public float projectileLifetime = 2f;

    [SerializeField] Animator playerAnims;

    private PlayerInputs playerInputs;
    private bool isVacuuming = false;

    private bool canAttack = false;

    [Header("Auto Aim")]
    public float autoAimRadius = 3f;
    public LayerMask enemyLayer;
    public GameObject aimMarkerPrefab;

    private Transform currentTarget;
    private GameObject currentMarker;


    private void Awake()
    {
        playerInputs = new PlayerInputs();

        playerInputs.Gameplay.Vaccum.performed += ctx => StartVacuum();
        playerInputs.Gameplay.Vaccum.canceled += ctx => StopVacuum();
        playerInputs.Gameplay.Attack.performed += ctx => Attack();
        playerInputs.Gameplay.Scope.performed += ctx => SwitchTarget();
    }
    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    public void ToggleAttack(bool attack)
    {
       canAttack = attack;
    }

    private void Update()
    {
        if (isVacuuming)
        {
            Suction();
        }

        if (currentTarget != null)
        {
            if (Vector2.Distance(transform.position, currentTarget.position) > autoAimRadius || currentTarget == null)
            {
                ClearTarget(); // Se sale del rango o fue destruido
            }
            else if (currentMarker != null)
            {
                ToggleAttack(true);
                currentMarker.transform.position = currentTarget.position + Vector3.up * 1.5f;
            }
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

                if (Vector2.Distance(rb.position, suctionPoint.position) < playerObjectVaccumDistance)
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
        if (!canAttack) return;

        AmmoSlot activeAmmo = collectibleManager.ammoSlots[collectibleManager.activeSlotIndex];
        if (activeAmmo == null || activeAmmo.IsEmpty() || activeAmmo.count <= 0) 
        {
            Debug.LogWarning("No active Ammo, Is Active ammo Empty, Ammo count <= 0");
            return;
        }


        GameObject prefabToShoot = activeAmmo.projectilePrefab;
        if (prefabToShoot == null)
        {
            Debug.LogWarning("No prefab activeAmmo proyectile");
            return;
        }

        // 🔹 Dirección de disparo
        Vector2 shootDirection;

        // 🔹 Flip del sprite si cambia de dirección
        if (currentTarget != null)
        {
            shootDirection = (currentTarget.position - transform.position).normalized;
        }
        else
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            shootDirection = (mouseWorldPos - transform.position).normalized;
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

    private void SwitchTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, autoAimRadius, enemyLayer);

        if (enemies.Length == 0)
        {
            ClearTarget();
            return;
        }

        // 🔹 Buscar el más cercano distinto del actual
        Transform nextTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (var e in enemies)
        {
            if (e.transform == currentTarget) continue;
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nextTarget = e.transform;
            }
        }

        if (nextTarget != null)
            SetTarget(nextTarget);

        ToggleAttack(true);
    }

    private void SetTarget(Transform target)
    {   
        currentTarget = target;
        ToggleAttack(true);
        if (currentMarker == null && aimMarkerPrefab != null)
            currentMarker = Instantiate(aimMarkerPrefab, target.position + Vector3.up * 1.5f, Quaternion.identity);
        else if (currentMarker != null)
            currentMarker.transform.position = target.position + Vector3.up * 1.5f;
    }

    private void ClearTarget()
    {
        currentTarget = null;

        if (currentMarker != null)
            Destroy(currentMarker);

        ToggleAttack(false);
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
