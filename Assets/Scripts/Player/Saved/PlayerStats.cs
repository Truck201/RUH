using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stats Principales")]
    public int vidas = 5;
    public float estamina = 1f;

    [Header("Nivel actual del jugador (0-5)")]
    [Range(0, 5)] public int nivelActual;

    [Header("UI Level Up")]
    [SerializeField] private GameObject canvasLevelUp;
    [SerializeField] TMP_Text canvasTitleText;
    [SerializeField] TMP_Text canvasLevelText;
    [SerializeField] TMP_Text levelCountActual;

    [Header("Experiencia del Jugador")]
    public float experienciaLevel = 50;
    public int experiencia = 0;
    public Image experienciaImage;

    public int woodCount = 0;
    public int metalCount = 0;

    [Header("Bool Level UP")]
    public bool levelUP = false;
    public bool levelCanvasActive = false;

    [Header("References")]
    [SerializeField] DialogueSystem dialogue;
    [SerializeField] blockAccess blocker;
    [SerializeField] DeliverManager deliverManager;
    [SerializeField] DeliverUIManager deliverManagerUI;

    [Header("En Caverna?")]
    public bool isInCavern = false;

    [Header("Train Repaired?")]
    public bool IsTrainRepaired = false;

    [Header("Inventario de Objetos")]
    public Dictionary<string, int> objetosAbsorbidos = new Dictionary<string, int>();

    // 👇 NUEVO: Guardar posición
    private Vector3 lastSavedPosition;
    private string lastSceneName;
    private Transform playerTransform;
    private Transform cameraTransform;

    private string[] CanvasLevelTitle = new string[]
    {
        "Bien",
        "Grandioso",
        "Genial",
        "Que Grande",
        "WOOOW",
        "Increible"
    };

    private void Awake()
    {
        // Singleton persistente
        if (PlayerStats.Instance == null)
        {
            PlayerStats.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        blocker?.ActivateBlock();
    }

    public void SetCameraToPlayer(Transform cameraT) => cameraTransform = cameraT;

    public void AddQuestItem(string itemName)
    {
        if (itemName == "Wood") woodCount++;
        if (itemName == "Metal") metalCount++;

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_pickup);
        Debug.Log($"Recolectado {itemName} -> Wood:{woodCount} Metal:{metalCount}");
    }
    public bool HasRequiredTrainItems()
    {
        return woodCount >= 3 && metalCount >= 2;
    }

    public void ConsumeTrainItems()
    {
        woodCount -= 3;
        metalCount -= 2;
    }

    private void Update()
    {
        if (levelUP)
        {
            levelUP = false;
            nivelActual += 1;
            experiencia = 0;
            experienciaLevel *= 1.3f;
            deliverManager.SetPedidosPorNivel(nivelActual);
            deliverManagerUI.MostrarPedidos();

            if (nivelActual == 2 && !dialogue.canAccessCavern)
            {
                dialogue.canAccessCavern = true;
                dialogue.NextDialogue();
                blocker?.DisableBlock();
            }

            if (SoundController.Instance != null)
                SoundController.Instance.PlaySFX(SoundController.Instance.SFX_newLevel);

            Debug.Log($"|| Level UP {nivelActual} ||");

            if (deliverManager)       
                deliverManager.ClearCompletedPedidos();

            // 👇 Nuevo: Mostrar Canvas de Level Up
            if (canvasLevelUp && !levelCanvasActive)
            {
                StartCoroutine(MostrarLevelUpCanvas());
            }
        }
    }

    // 🔹 Corrutina: muestra el canvas 2.5 segundos
    private IEnumerator MostrarLevelUpCanvas()
    {
        levelCanvasActive = true;

        if (canvasLevelUp) canvasLevelUp.SetActive(true);

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_newLevel);

        if (canvasLevelText) canvasLevelText.text = $"Nivel {nivelActual}";

        int range = UnityEngine.Random.Range(0, CanvasLevelTitle.Length);

        if (canvasTitleText) canvasTitleText.SetText($"¡{CanvasLevelTitle[range]}!");

        if (levelCountActual) levelCountActual.SetText($"{nivelActual}");

        yield return new WaitForSeconds(2.5f);

        if (canvasLevelUp) canvasLevelUp.SetActive(false);

        levelCanvasActive = false;
    }

    // 🔹 Agregar experiencia
    public void AddExperience(int amount)
    {
        experiencia += amount;

        if (experiencia >= experienciaLevel && nivelActual <= 4)
        {
            levelUP = true;
        }

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_winExperience);

        experienciaImage.fillAmount = experiencia / experienciaLevel;
        Debug.Log($"|| Experiencia total: {experiencia} ||");
    }

    // 🔹 Agregar objeto
    public void AddObjeto(string nombre, int cantidad = 1)
    {
        if (!objetosAbsorbidos.ContainsKey(nombre))
            objetosAbsorbidos[nombre] = 0;

        objetosAbsorbidos[nombre] += cantidad;

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_pickup);

        Debug.Log($"Objeto {nombre}: {objetosAbsorbidos[nombre]}");
    }

    // 🔹 Quitar objeto (solo si hay stock)
    public bool RemoveObjeto(string nombre, int cantidad = 1)
    {
        if (!objetosAbsorbidos.ContainsKey(nombre) || objetosAbsorbidos[nombre] < cantidad)
            return false;

        objetosAbsorbidos[nombre] -= cantidad;
        Debug.Log($"Usado {cantidad} {nombre}. Quedan: {objetosAbsorbidos[nombre]}");
        return true;
    }

    // 🔹 Obtener cantidad
    public int GetObjetoCount(string nombre)
    {
        return objetosAbsorbidos.ContainsKey(nombre) ? objetosAbsorbidos[nombre] : 0;
    }


    public void AddStone(string stoneName, GameObject projectilePrefab, Sprite icon, int amount = 1)
    {
        AddObjeto(stoneName, amount);
    }

    public bool UseStone(string stoneName, int amount = 1)
    {
        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_shoot);

        return RemoveObjeto(stoneName, amount);
    }

    public void AddVeggie(string veggieName, Sprite icon, int amount = 1)
    {
        AddObjeto(veggieName, amount);
        CollectibleManager.Instance.UpdateVeggieUI(veggieName, icon, GetObjetoCount(veggieName));
    }

    public int GetStoneCount(string stoneName) => GetObjetoCount(stoneName);
    public int GetVeggieCount(string veggieName) => GetObjetoCount(veggieName);

    // 🔹 Verificar si se puede entregar un pedido
    public bool PuedeCompletarPedido(ComunityDeliver.DeliverLevel pedido)
    {
        if (GetObjetoCount("Zanahoria") < pedido.zanahorias) return false;
        if (GetObjetoCount("Papa") < pedido.papas) return false;
        if (GetObjetoCount("Cebolla") < pedido.cebollas) return false;
        return true;
    }

    // 🔹 Entregar pedido (descuenta ingredientes y suma exp)
    public void EntregarPedido(ComunityDeliver.DeliverLevel pedido)
    {
        if (!PuedeCompletarPedido(pedido))
        {
            Debug.LogWarning("No tienes suficientes ingredientes para este pedido.");
            return;
        }

        RemoveObjeto("Zanahoria", pedido.zanahorias);
        RemoveObjeto("Papa", pedido.papas);
        RemoveObjeto("Cebolla", pedido.cebollas);

        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance.UpdateVeggieUI("Zanahoria", null, GetObjetoCount("Zanahoria"));
            CollectibleManager.Instance.UpdateVeggieUI("Papa", null, GetObjetoCount("Papa"));
            CollectibleManager.Instance.UpdateVeggieUI("Cebolla", null, GetObjetoCount("Cebolla"));
        }

        if (SoundController.Instance != null)
            SoundController.Instance.PlaySFX(SoundController.Instance.SFX_delivered);

        AddExperience(pedido.experiencia);
        Debug.Log("Pedido entregado correctamente.");
    }

    public void ReferencesObjects()
    {
        if (dialogue == null)
            dialogue = Object.FindAnyObjectByType<DialogueSystem>();

        if (blocker == null)
            blocker = Object.FindAnyObjectByType<blockAccess>();

        if (deliverManager == null)
            deliverManager = Object.FindAnyObjectByType<DeliverManager>();

        if (deliverManagerUI == null)
            deliverManagerUI = Object.FindAnyObjectByType<DeliverUIManager>();

        if (!canvasLevelUp) // GameObject
        {
            GameObject go = GameObject.Find("LevelUpCanvas");
            Debug.Log("Canvas Level Up");
            if (go) canvasLevelUp = go;
        }

        if (!canvasTitleText)
        {
            GameObject go = GameObject.Find("Comentario");
            Debug.Log("Canvas Title Text");
            if (go) canvasTitleText = go.GetComponent<TMP_Text>();
        }

        if (!canvasLevelText)
        {
            GameObject go = GameObject.Find("LevelText");
            Debug.Log("Canvas Level Text");
            if (go) canvasLevelText = go.GetComponent<TMP_Text>();
        }

        if (!levelCountActual)
        {
            GameObject go = GameObject.Find("LevelCount");
            Debug.Log("Level Count Actual");
            if (go) levelCountActual = go.GetComponent<TMP_Text>();
        }

        if (!experienciaImage)
        {
            GameObject go = GameObject.Find("Experiencia");
            Debug.Log("Experiencia Image");
            if (go) experienciaImage = go.GetComponent<Image>();
        }

        if (canvasLevelUp) canvasLevelUp.SetActive(false);
        if (canvasTitleText) canvasTitleText.SetText($"¡{CanvasLevelTitle[0]}!");
        if (levelCountActual) levelCountActual.SetText($"{nivelActual}");
        if (experienciaImage) experienciaImage.fillAmount = experiencia / experienciaLevel;
    }

}
