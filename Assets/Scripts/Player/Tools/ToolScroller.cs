using UnityEngine;
using UnityEngine.UI;

public class ToolScroller : MonoBehaviour
{
    public AmmoSlot[] ammoSlots = new AmmoSlot[3];

    [SerializeField] private Image slot1Icon;
    [SerializeField] private Image slot2Icon;
    [SerializeField] private Image slot3Icon;
    [SerializeField] private TMPro.TextMeshProUGUI slot1Count;
    [SerializeField] private TMPro.TextMeshProUGUI slot2Count;
    [SerializeField] private TMPro.TextMeshProUGUI slot3Count;

    [SerializeField] RectTransform slot_1;
    [SerializeField] RectTransform slot_2;
    [SerializeField] RectTransform slot_3;

    [Header("Carrusel Settings")]
    [SerializeField] private Vector3 leftPos;
    [SerializeField] private Vector3 centerPos;
    [SerializeField] private Vector3 rightPos;

    [SerializeField] private Vector3 leftScale = new Vector3(0.2f, 0.8f, 1f);
    [SerializeField] private Vector3 centerScale = new Vector3(0.3f, 1f, 1f);
    [SerializeField] private Vector3 rightScale = new Vector3(0.2f, 0.8f, 1f);

    public int activeSlotIndex = 0;

    [SerializeField] float animTime = 0.25f;

    [Header("Input Delay")]
    public float scrollDelay = 0.3f;   // Tiempo mínimo entre scrolls
    private float scrollTimer = 0f;

    [Header("Default Slot")]
    [SerializeField] private AmmoSlot slotDefault;

    private PlayerInputs playerInputs;

    void Awake()
    {
        playerInputs = new PlayerInputs();
        playerInputs.Gameplay.Scroll.performed += ctx => OnScroll(ctx.ReadValue<float>());
    }
    void OnEnable()
    {
        playerInputs.Enable();
        InitializeSlots();
        UpdateUI();
        scrollTimer = 0f;
    }

    void OnDisable()
    {
        playerInputs.Disable();
    }
    private void InitializeSlots()
    {
        // Asignar posiciones y tamaños iniciales
        slot_1.anchoredPosition = leftPos;
        slot_1.localScale = leftScale;

        slot_2.anchoredPosition = centerPos;
        slot_2.localScale = centerScale;

        slot_3.anchoredPosition = rightPos;
        slot_3.localScale = rightScale;
    }
    void Update()
    {
        // Contador de tiempo
        if (scrollTimer > 0f)
            scrollTimer -= Time.deltaTime;
    }
    private void OnScroll(float value)
    {
        if (scrollTimer > 0f) return;

        if (value > 0)
        {
            ScrollLeft();
            activeSlotIndex = (activeSlotIndex + 1) % ammoSlots.Length;
        }
        //else if (value < 0)
        //{
        //    ScrollRight();
        //    activeSlotIndex = (activeSlotIndex - 1 + ammoSlots.Length) % ammoSlots.Length;
        //}

        UpdateUI();
        scrollTimer = scrollDelay;
    }

    //private void ScrollRight()
    //{
    //    // Animar posiciones
    //    LeanTween.move(slot_1, centerPos, animTime);
    //    LeanTween.move(slot_2, rightPos, animTime);
    //    LeanTween.move(slot_3, leftPos, animTime);

    //    // Animar tamaños
    //    LeanTween.scale(slot_1, centerScale, animTime);
    //    LeanTween.scale(slot_2, rightScale, animTime);
    //    LeanTween.scale(slot_3, leftScale, animTime);

    //    // Rotar referencias
    //    RectTransform temp = slot_3;
    //    slot_3 = slot_2;
    //    slot_2 = slot_1;
    //    slot_1 = temp;
    //}
    private void ScrollLeft()
    {
        // Animar posiciones
        LeanTween.move(slot_1, rightPos, animTime);
        LeanTween.move(slot_2, leftPos, animTime);
        LeanTween.move(slot_3, centerPos, animTime);

        // Animar tamaños
        LeanTween.scale(slot_1, rightScale, animTime);
        LeanTween.scale(slot_2, leftScale, animTime);
        LeanTween.scale(slot_3, centerScale, animTime);

        // Rotar referencias
        RectTransform temp = slot_1;
        slot_1 = slot_2;
        slot_2 = slot_3;
        slot_3 = temp;
    }
    public void AddAmmo(string veggieName, Sprite veggieIcon, GameObject projectilePrefab, int amount)
    {
        // 1) Primero, intentar sumar si ya existe en algún slot de UI
        for (int i = 0; i < ammoSlots.Length; i++)
        {
            AmmoSlot slot = ammoSlots[i];
            if (!slot.IsEmpty() && slot.name == veggieName)
            {
                slot.count += amount;
                PlayerStats.Instance.AddObjeto(veggieName, amount);
                UpdateUI();
                return;
            }
        }

        // 2) Si no existe, intentar añadir en slot activo
        int startIndex = activeSlotIndex;
        for (int offset = 0; offset < ammoSlots.Length; offset++)
        {
            int i = (startIndex + offset) % ammoSlots.Length;
            AmmoSlot slot = ammoSlots[i];

            if (slot.IsEmpty())
            {
                slot.name = veggieName;
                slot.projectilePrefab = projectilePrefab;
                slot.count = amount;
                slot.icon = veggieIcon;

                PlayerStats.Instance.AddObjeto(veggieName, amount);
                UpdateUI();
                return;
            }
        }

        // 3) Si todos los slots están ocupados
        Debug.LogWarning($"No hay slots disponibles para {veggieName}");
    }

    public bool UseAmmo()
    {
        AmmoSlot slot = ammoSlots[activeSlotIndex];

        if (slot.count > 0)
        {
            slot.count--;
            PlayerStats.Instance.RemoveObjeto(slot.name, 1);

            if (slot.count <= 0)
            {
                slot.Clear(); // limpiamos el slot
            }

            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        // Slot 0 → default si está vacío
        AmmoSlot slot0 = (ammoSlots.Length > 0 && !ammoSlots[0].IsEmpty() && ammoSlots[0].count > 0)
            ? ammoSlots[0]
            : slotDefault;

        // Slots 1 y 2 → solo si existen y tienen contenido
        AmmoSlot slot1 = (ammoSlots.Length > 1 && !ammoSlots[1].IsEmpty() && ammoSlots[1].count > 0)
            ? ammoSlots[1]
            : null;

        AmmoSlot slot2 = (ammoSlots.Length > 2 && !ammoSlots[2].IsEmpty() && ammoSlots[2].count > 0)
            ? ammoSlots[2]
            : null;

        // Actualizamos UI
        UpdateSlotUI(slot1Icon, slot1Count, slot0);
        UpdateSlotUI(slot2Icon, slot2Count, slot1);
        UpdateSlotUI(slot3Icon, slot3Count, slot2);
    }

    private void UpdateSlotUI(Image icon, TMPro.TextMeshProUGUI countText, AmmoSlot slot)
    {
        if (slot == null || slot.IsEmpty() || slot.count <= 0)
        {
            icon.sprite = slotDefault.spriteEmpty;
            countText.text = "--"; // siempre slot default si está vacío
        }
        else
        {
            icon.sprite = slot.GetSpriteForCount();
            countText.text = slot.count.ToString("D2");
        }
    }


    public bool CanAddAmmo(string ammoName)
    {
        // Verificar si ya existe
        foreach (var slot in ammoSlots)
        {
            if (!slot.IsEmpty() && slot.name == ammoName)
                return true;
        }

        // Verificar si hay algún slot vacío
        foreach (var slot in ammoSlots)
        {
            if (slot.IsEmpty())
                return true;
        }

        return false; // Todos ocupados y diferentes
    }

}
