using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;

    [Header("Ammo Slot")]
    public AmmoSlot[] ammoSlots = new AmmoSlot[1];

    [Header("Ammo UI")]
    [SerializeField] private Image slotIconAmmo;
    [SerializeField] private TMP_Text slotCountAmmo;
    [SerializeField] GameObject stoneContainer;

    [Header("Veggie UI")]
    [SerializeField] private Transform veggiePanel;
    [SerializeField] private GameObject veggieSlotPrefab;
    private Dictionary<string, TMP_Text> veggieUI = new();
    private Dictionary<string, GameObject> veggieSlots = new();

    [SerializeField] private Sprite defaultEmptySprite;

    public int activeSlotIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (ammoSlots == null || ammoSlots.Length == 0)
            ammoSlots = new AmmoSlot[1];

        for (int i = 0; i < ammoSlots.Length; i++)
        {
            if (ammoSlots[i] == null)
                ammoSlots[i] = new AmmoSlot();
        }

        UpdateUI();
    }

    public void UpdateAmmoUI(string stoneName, int count, Sprite icon)
    {
        AmmoSlot slot = ammoSlots[0];
        slot.name = stoneName;
        slot.count = count;
        slot.icon = icon;
        UpdateUI();
    }

    public void UpdateVeggieUI(string veggieName, Sprite icon, int newAmount)
    {
        if (!veggieUI.ContainsKey(veggieName))
        {
            Debug.Log("No existe la verdura en UI");
            if (veggieSlotPrefab != null && veggiePanel != null)
            {
                GameObject newSlot = Instantiate(veggieSlotPrefab, veggiePanel);
                veggieSlots[veggieName] = newSlot;

                Image iconImage = newSlot.transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                    iconImage.sprite = icon;

                TMP_Text countText = newSlot.transform.Find("Count")?.GetComponent<TMP_Text>();
                if (countText != null) veggieUI[veggieName] = countText;

                Debug.Log("Creado Nuevo Verdura");
            }
        }

        if (veggieUI.ContainsKey(veggieName))
            veggieUI[veggieName].text = newAmount.ToString("D2");

        if (veggieSlots.ContainsKey(veggieName))
            veggieSlots[veggieName].SetActive(newAmount > 0);

        Debug.Log("Añadir textos y Activar si es mayor a 0");
        Debug.Log($"nombre {veggieName} y cantidad {newAmount}");
        Debug.Log($"Slot Total {veggieSlots}");
    }

    private void UpdateUI()
    {
        AmmoSlot slot = ammoSlots.Length > 0 ? ammoSlots[0] : null;
        if (slot == null || slot.IsEmpty() || slot.count <= 0)
        {
            if (slotIconAmmo != null) slotIconAmmo.sprite = defaultEmptySprite;
            if (slotCountAmmo != null) slotCountAmmo.text = "--";
            stoneContainer.SetActive(false);
            return;
        }

        stoneContainer.SetActive(true);

        if (slotIconAmmo != null) slotIconAmmo.sprite = slot.GetSpriteForCount() ?? slot.icon ?? defaultEmptySprite;
        if (slotCountAmmo != null) slotCountAmmo.text = slot.count.ToString("D2");
    }
}
