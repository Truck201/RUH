using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WhirlwindWeapon : MonoBehaviour
{
    [Header("Vacuum Settings")]
    public Transform suctionPoint; // donde las piedras se �pegan�
    public float suctionForce = 5f;
    public LayerMask stoneLayer;

    private List<Collider2D> objectsInsideSuction = new List<Collider2D>();

    [Header("Vaccum Player - Object distance")]
    public float playerObjectVaccumDistance = 0.4f;

    [SerializeField] PlayerMovement playerManager;
    [SerializeField] CollectibleManager collectibleManager;

    [Header("Attack Settings")]
    public float shootForce = 10f;
    public float projectileLifetime = 2f;

    [SerializeField] Animator playerAnims;
    private Animator weaponAnimator;

    private PlayerInputs playerInputs;
    private bool isVacuuming = false;
    public bool frontVaccum;
    public bool backVaccum;

    private bool canAttack = false;

    [Header("Auto Aim")]
    public float autoAimRadius = 3f;
    public LayerMask enemyLayer;
    public GameObject aimMarkerPrefab;

    private Transform currentTarget;
    private GameObject currentMarker;

    private DynamicSortingOrder sortingOrder;

    private void Awake()
    {
        playerInputs = new PlayerInputs();

        playerInputs.Gameplay.Vaccum.performed += ctx => StartVacuum();
        playerInputs.Gameplay.Vaccum.canceled += ctx => StopVacuum();
        playerInputs.Gameplay.Attack.performed += ctx => Attack();
        playerInputs.Gameplay.Scope.performed += ctx => SwitchTarget();

        weaponAnimator = GetComponent<Animator>();

        sortingOrder = GetComponent<DynamicSortingOrder>();
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

        if (sortingOrder != null)
        {
            if (frontVaccum)
            {
                sortingOrder.offset = 1f; // Se va al fondo
            }
            else
            {
                sortingOrder.offset = -1f; // Vuelve al frente normal
            }
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

        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("Vaccum", isVacuuming);
            weaponAnimator.SetBool("Front", frontVaccum);
            weaponAnimator.SetBool("Back", backVaccum);

            if (isVacuuming)
            {
                playerAnims.SetBool("isVaccum", true);
            } else
            {
                playerAnims.SetBool("isVaccum", false);
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

        foreach (var hit in objectsInsideSuction)
        {
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_noAbsorb);
    }
    private void Suction()
    {
        foreach (var hit in objectsInsideSuction.ToArray())
        {
            if (hit == null) { objectsInsideSuction.Remove(hit); continue; }

            Rigidbody2D rb = hit.attachedRigidbody;

            if (rb == null) continue;

            Vector2 dir = ((Vector2)suctionPoint.position - rb.position).normalized;
            rb.linearVelocity = dir * suctionForce;

            if (Vector2.Distance(rb.position, suctionPoint.position) < playerObjectVaccumDistance)
            {
                Collectible c = hit.GetComponent<Collectible>();
                if (c != null)
                {
                    playerAnims.SetTrigger("Absorb");
                    switch (c.type)
                    {
                        case CollectibleType.Stone:
                            PlayerStats.Instance.AddStone(c.itemName, c.projectilePrefab, c.itemIcon);
                            CollectibleManager.Instance.UpdateAmmoUI(
                                c.itemName,
                                PlayerStats.Instance.GetStoneCount(c.itemName),
                                c.itemIcon
                            );
                            break;
                        case CollectibleType.Veggie:
                            PlayerStats.Instance.AddVeggie(c.itemName, c.itemIcon);
                            break;

                        case CollectibleType.QuestItem:
                            PlayerStats.Instance.AddQuestItem(c.itemName);
                            break;
                    }
                }

                Destroy(rb.gameObject);
                objectsInsideSuction.Remove(hit);
            }
        }
    }

    public void Attack()
    {
        if (!canAttack) return;

        AmmoSlot slot = CollectibleManager.Instance.ammoSlots[0];
        if (slot == null || slot.IsEmpty() || PlayerStats.Instance.GetStoneCount(slot.name) <= 0) return;

        GameObject projectile = Instantiate(slot.projectilePrefab, suctionPoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        Vector2 shootDirection;

        // ✅ Si hay objetivo (scope auto-aim), disparamos hacia él
        if (currentTarget != null)
        {
            shootDirection = (currentTarget.position - suctionPoint.position).normalized;
        }
        else
        {
            // ✅ Si NO hay objetivo, disparamos hacia donde mira el jugador / arma
            shootDirection = transform.right; // O usa transform.up si tu mira es vertical
        }

        rb.linearVelocity = shootDirection * shootForce;


        StoneProjectile sp = projectile.GetComponent<StoneProjectile>();
        sp.explosionDelay = projectileLifetime;
        sp.isProyectible = true;

        PlayerStats.Instance.UseStone(slot.name);
        CollectibleManager.Instance.UpdateAmmoUI(slot.name, PlayerStats.Instance.GetStoneCount(slot.name), slot.icon);

        if (playerAnims) playerAnims.SetTrigger("Shoot");
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
            //if (e.transform == currentTarget) continue;
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nextTarget = e.transform;
            }
        }

        if (nextTarget == currentTarget)
        {
            ClearTarget();
            return;
        }

        if (nextTarget != null)
            SetTarget(nextTarget);

        ToggleAttack(true);
    }

    private void SetTarget(Transform target )
    {   
        currentTarget = target;
        ToggleAttack(true);
        if (currentMarker == null && aimMarkerPrefab != null)
            currentMarker = Instantiate(aimMarkerPrefab, target.position + Vector3.up * 1.5f, Quaternion.identity);
        else if (currentMarker != null)
            currentMarker.transform.position = target.position + Vector3.up * 1.5f;

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_select);
    }

    private void ClearTarget()
    {
        currentTarget = null;

        if (currentMarker != null)
            Destroy(currentMarker);

        ToggleAttack(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & stoneLayer) != 0)
        {
            if (!objectsInsideSuction.Contains(other))
                objectsInsideSuction.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (objectsInsideSuction.Contains(other))
            objectsInsideSuction.Remove(other);
    }
}
