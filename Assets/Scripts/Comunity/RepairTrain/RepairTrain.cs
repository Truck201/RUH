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

    private bool trainRepaired = false;

    public Sprite woodSprite; // Icono de madera
    public Sprite metalSprite; // Icono de metal

    [Header("References")]
    [SerializeField] DialogueSystem dialogue;

    private SpriteRenderer spriteRenderer;
    private GameObject player;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        entregaButtonUI.SetActive(false);
        resourcesUI.SetActive(false);
    }

    private void Start()
    {
        if (PlayerStats.Instance.IsTrainRepaired)
        {
            startRepaired();
        }
    }

    private void Update()
    {
        if (PlayerStats.Instance.IsTrainRepaired)  return;

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
        PlayerStats.Instance.IsTrainRepaired = true;

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_repairTrain);

        spriteRenderer.sprite = spriteRepaired;
        entregaButtonUI.SetActive(false);
        resourcesUI.SetActive(false);

        dialogue.NextDialogue();

        Debug.Log("🚂 Tren reparado — ahora se pueden entregar pedidos.");

        // ✅ Mostrar pedidos en la UI
        var uiManager = FindFirstObjectByType<DeliverUIManager>();
        if (uiManager != null)
            uiManager.MostrarPedidos();
    }
    
    private void startRepaired()
    {
        spriteRenderer.sprite = spriteRepaired;
        entregaButtonUI.SetActive(false);
        resourcesUI.SetActive(false);

        var uiManager = FindFirstObjectByType<DeliverUIManager>();
        uiManager.ContainerReferences();
        if (uiManager != null)
            uiManager.MostrarPedidos();
    }
}
