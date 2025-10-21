using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RepairTrain : MonoBehaviour
{
    [Header("Sprites del tren")]
    public Sprite spriteStart;     // Tren roto
    public Sprite spriteRepaired;  // Tren reparado

    [Header("Interacción")]
    public float interactRadius = 2f;
    public GameObject entregaButtonUI;  // UI del botón "Entregar"

    [Header("UI Recursos en el tren")]
    public GameObject resourcesUI;   // ← Padre del UI (Wood + Metal)
    public TMP_Text woodTextUI;
    public TMP_Text metalTextUI;
    public Image woodIconUI;
    public Image metalIconUI;

    public Sprite woodSprite; // Icono de madera
    public Sprite metalSprite; // Icono de metal

    public static bool IsTrainRepaired = false;

    private SpriteRenderer spriteRenderer;
    private GameObject player;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        entregaButtonUI.SetActive(false);
        resourcesUI.SetActive(false);
    }

    private void Update()
    {
        if (IsTrainRepaired) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        // ✅ Si el jugador está cerca, mostrar botón de interacción
        if (distance <= interactRadius)
        {
            entregaButtonUI.SetActive(true);
            MostrarRecursosUI();

            // ✅ Si presiona el botón de "Entregar" → reparar tren
            if (GlobalInputManager.Instance.DeliverPressed())
            {
                if (PlayerStats.Instance.HasRequiredTrainItems())
                {
                    RepararTren();
                    if (SoundController.Instance != null)
                        SoundController.Instance.PlaySFX(SoundController.Instance.SFX_delivered);
                }
                else
                {
                    Debug.Log("❌ No tienes suficientes materiales (3 madera y 2 metal) para reparar el tren.");
                    if (SoundController.Instance != null)
                        SoundController.Instance.PlaySFX(SoundController.Instance.SFX_cannotDeliver);
                }
            }
        }
        else
        {
            entregaButtonUI.SetActive(false);
            resourcesUI.SetActive(false);
        }
    }

    private void MostrarRecursosUI()
    {
        resourcesUI.SetActive(true);

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_claxonTrain);

        // Asignar íconos
        if (woodIconUI) woodIconUI.sprite = woodSprite;
        if (metalIconUI) metalIconUI.sprite = metalSprite;

        // Mostrar cantidad actual
        woodTextUI.text = PlayerStats.Instance.woodCount + " / 3";
        metalTextUI.text = PlayerStats.Instance.metalCount + " / 2";
    }

    private void RepararTren()
    {
        // ✅ Consumir recursos
        PlayerStats.Instance.ConsumeTrainItems();

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_repairTrain);

        IsTrainRepaired = true;
        spriteRenderer.sprite = spriteRepaired;
        entregaButtonUI.SetActive(false);
        resourcesUI.SetActive(false);

        Debug.Log("🚂 Tren reparado — ahora se pueden entregar pedidos.");

        // ✅ Mostrar pedidos en la UI
        var uiManager = FindFirstObjectByType<DeliverUIManager>();
        if (uiManager != null)
            uiManager.MostrarPedidos();
    }
}
