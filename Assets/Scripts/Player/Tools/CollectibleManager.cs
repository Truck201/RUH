using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;
    [Header("Ammo Slot")]
    public AmmoSlot[] ammoSlots = new AmmoSlot[1];

    [Header("Ammo Settings")]
    [SerializeField] private Image slotIconAmmo;
    [SerializeField] private TMPro.TextMeshProUGUI slotCountAmmo;
    [SerializeField] RectTransform slotAmmo;

    [Header("Pos Ammo")]
    [SerializeField] private Vector3 topPos;

    [Header("Veggie UI")]
    [SerializeField] private Transform veggiePanel;
    [SerializeField] private GameObject veggieSlotPrefab;
    private Dictionary<string, TMPro.TextMeshProUGUI> veggieUI = new();
    private Dictionary<string, int> veggieInventory = new Dictionary<string, int>();
    private Dictionary<string, GameObject> veggieSlots = new(); // guarda la instancia real del slot

    public int activeSlotIndex = 0;

    [Header("Sprites por defecto")]
    [SerializeField] private Sprite defaultEmptySprite;

    private void Awake()
    {
        // Singleton persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    void OnEnable()
    {
        if (ammoSlots == null || ammoSlots.Length == 0)
            ammoSlots = new AmmoSlot[1];

        for (int i = 0; i < ammoSlots.Length; i++)
        {
            if (ammoSlots[i] == null)
                ammoSlots[i] = new AmmoSlot(); // crea uno por defecto
        }

        InitializeSlots();
        UpdateUI();
    }

    private void InitializeSlots()
    {
        if (slotAmmo != null)
            slotAmmo.anchoredPosition = topPos;
    }

    public void AddAmmo(string stoneName, Sprite stoneIcon, GameObject projectilePrefab, int amount)
    {
        // 1) Primero, intentar sumar si ya existe en algún slot de UI
        for (int i = 0; i < ammoSlots.Length; i++)
        {
            AmmoSlot slot = ammoSlots[i];
            if (!slot.IsEmpty() && slot.name == stoneName)
            {
                slot.count += amount;
                UpdateUI();
                return;
            }
        }

        // 2) Si no existe, intentar añadir en slot activo
        int startIndex = Mathf.Clamp(activeSlotIndex, 0, ammoSlots.Length - 1);
        for (int offset = 0; offset < ammoSlots.Length; offset++)
        {
            int i = (startIndex + offset) % ammoSlots.Length;
            AmmoSlot slot = ammoSlots[i];

            if (slot.IsEmpty())
            {
                slot.name = stoneName;
                slot.projectilePrefab = projectilePrefab;
                slot.count = amount;
                slot.icon = stoneIcon;

                UpdateUI();
                return;
            }
        }

        // 3) Si todos los slots están ocupados
        Debug.LogWarning($"No hay slots disponibles para {stoneName}");
    }

    public void AddVeggie(string veggieName, Sprite veggieIcon, int amount)
    {
        if (!veggieInventory.ContainsKey(veggieName))
        {
            veggieInventory[veggieName] = 0;

            // Instanciar UI dinámico
            if (veggieSlotPrefab != null && veggiePanel != null)
            {
                GameObject newSlot = Instantiate(veggieSlotPrefab, veggiePanel);
                veggieSlots[veggieName] = newSlot;

                var icon = newSlot.transform.Find("Icon")?.GetComponent<Image>();
                if (icon != null) icon.sprite = veggieIcon;

                TMPro.TextMeshProUGUI countText = newSlot.transform.Find("Count")?.GetComponent<TMPro.TextMeshProUGUI>();
                if (countText != null)
                    veggieUI[veggieName] = countText;
                else
                    Debug.LogWarning("Veggie slot prefab necesita un TextMeshProUGUI llamado 'Count'");
            }
            else
            {
                Debug.LogWarning("veggieSlotPrefab o veggiePanel no asignados en CollectibleManager.");
            }
        }

        veggieInventory[veggieName] += amount;
        PlayerStats.Instance.AddObjeto(veggieName, amount);
        if (veggieUI.ContainsKey(veggieName))
            veggieUI[veggieName].text = veggieInventory[veggieName].ToString("D2");

        // 👇 si llega a 0 o menos, ocultar el slot
        if (veggieSlots.ContainsKey(veggieName))
            veggieSlots[veggieName].SetActive(veggieInventory[veggieName] > 0);

        Debug.Log($"🥕 {veggieName} agregado. Total = {veggieInventory[veggieName]}");
    }

    public bool UseAmmo()
    {
        if (ammoSlots == null || ammoSlots.Length == 0) return false;

        AmmoSlot slot = ammoSlots[Mathf.Clamp(activeSlotIndex, 0, ammoSlots.Length - 1)];

        if (slot != null && slot.count > 0)
        {
            // quitar primero del PlayerStats (usa el nombre actual)
            PlayerStats.Instance.RemoveObjeto(slot.name, 1);

            // descontar la cuenta
            slot.count--;

            // si queda 0, limpiar el slot
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
            ? ammoSlots[0] : null;

        UpdateSlotUI(slotIconAmmo, slotCountAmmo, slot0);
    }

    private void UpdateSlotUI(Image icon, TMPro.TextMeshProUGUI countText, AmmoSlot slot)
    {
        // Manejar caso slot == null de forma segura
        if (slot == null || slot.IsEmpty() || slot.count <= 0)
        {
            // Si el slot es null no podemos acceder a slot.spriteEmpty -> usar defaultEmptySprite (o dejar vacío)
            if (slot != null && slot.spriteEmpty != null)
                icon.sprite = slot.spriteEmpty;
            else if (defaultEmptySprite != null)
                icon.sprite = defaultEmptySprite;
            else
                icon.sprite = null;

            if (countText != null)
                countText.text = "--";
            return;
        }

        // Selección de sprite según cantidad (si el slot tiene sprites configurados)
        if (slot.count >= 8 && slot.spriteCount8 != null)
            icon.sprite = slot.spriteCount8;
        else if (slot.count >= 5 && slot.spriteCount5 != null)
            icon.sprite = slot.spriteCount5;
        else if (slot.count >= 3 && slot.spriteCount3 != null)
            icon.sprite = slot.spriteCount3;
        else if (slot.count >= 1 && slot.spriteCount1 != null)
            icon.sprite = slot.spriteCount1;
        else if (slot.icon != null)
            icon.sprite = slot.icon;
        else if (defaultEmptySprite != null)
            icon.sprite = defaultEmptySprite;

        if (countText != null)
            countText.text = slot.count.ToString("D2");
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

    public void UpdateVeggieUI(string veggieName, int newAmount)
    {
        if (veggieInventory.ContainsKey(veggieName))
            veggieInventory[veggieName] = newAmount;
        else
            veggieInventory.Add(veggieName, newAmount);

        if (veggieUI.ContainsKey(veggieName))
            veggieUI[veggieName].text = veggieInventory[veggieName].ToString("D2");

        if (veggieSlots.ContainsKey(veggieName))
            veggieSlots[veggieName].SetActive(newAmount > 0);
    }
}
