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
    [SerializeField] TMP_Text canvasLevelText;
    [SerializeField] TMP_Text levelCountActual;

    [Header("Experiencia del Jugador")]
    public float experienciaLevel = 50;
    public int experiencia = 0;
    public Image experienciaImage;

    [Header("Bool Level UP")]
    public bool levelUP = false;
    public bool levelCanvasActive = false;

    [SerializeField] private DeliverManager deliverManager;
    [SerializeField] private DeliverUIManager deliverManagerUI;

    [Header("Inventario de Objetos")]
    public Dictionary<string, int> objetosAbsorbidos = new Dictionary<string, int>();

    // 👇 NUEVO: Guardar posición
    private Vector3 lastSavedPosition;
    private string lastSceneName;
    private Transform playerTransform;
    private Transform cameraTransform;

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

        if (canvasLevelUp) canvasLevelUp.SetActive(false);
        if (levelCountActual) levelCountActual.SetText($"{nivelActual}");
        if (experienciaImage) experienciaImage.fillAmount = experiencia / experienciaLevel;

        if (!deliverManager) FindFirstObjectByType(typeof(DeliverManager));
        if (!deliverManagerUI) FindFirstObjectByType(typeof(DeliverUIManager));

        // 👇 Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 🔹 Método público para asignar el Player
    public void SetPlayer(Transform player) => playerTransform = player;
    public void SetCameraToPlayer(Transform cameraT) => cameraTransform = cameraT;

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

        if (canvasLevelText) canvasLevelText.text = $"Nivel {nivelActual}";

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

        experienciaImage.fillAmount = experiencia / experienciaLevel;
        Debug.Log($"|| Experiencia total: {experiencia} ||");
    }

    // 🔹 Agregar objeto
    public void AddObjeto(string nombre, int cantidad = 1)
    {
        if (!objetosAbsorbidos.ContainsKey(nombre))
            objetosAbsorbidos[nombre] = 0;

        objetosAbsorbidos[nombre] += cantidad;
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

        AddExperience(pedido.experiencia);
        Debug.Log("Pedido entregado correctamente.");
    }


    // 🔹 Guardar posición actual antes de cambiar de escena
    public void GuardarPosicion(Vector3 playerPositionSave)
    {
        if (playerTransform == null) return;
        lastSavedPosition = playerPositionSave;   // playerTransform.position
        lastSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"📍 Posición guardada: {lastSavedPosition}");
    }

    // 🔹 Restaurar posición al cargar escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerTransform == null)
            return;

        // Solo restaurar si es la misma escena de donde saliste
        if (scene.name == lastSceneName && lastSavedPosition != Vector3.zero)
        {
            playerTransform.position = lastSavedPosition;
            cameraTransform.position = lastSavedPosition;
            Debug.Log($"📍 Posición restaurada: {playerTransform.position}");

            if (!deliverManager) FindFirstObjectByType(typeof(DeliverManager));
            if (!deliverManagerUI) FindFirstObjectByType(typeof(DeliverUIManager));
        }
    }
}
