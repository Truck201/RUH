using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stats Principales")]
    public int vidas = 5;
    public int estamina = 1;

    [Header("Nivel actual del jugador (0-5)")]
    [Range(0, 5)]
    public int nivelActual;

    [Header("Experiencia del Jugador")]
    public float experienciaLevel = 50;
    public int experiencia = 0;

    [Header("Bool Level UP")]
    public bool levelUP = false;

    [SerializeField] private DeliverManager deliverManager;
    [SerializeField] private DeliverUIManager deliverManagerUI;

    [Header("Inventario de Objetos")]
    public Dictionary<string, int> objetosAbsorbidos = new Dictionary<string, int>();

    private void Awake()
    {
        // Singleton persistente
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
        }
    }

    // 🔹 Agregar experiencia
    public void AddExperience(int amount)
    {
        experiencia += amount;

        if (experiencia >= experienciaLevel)
        {
            if (nivelActual <= 4)
            {
                levelUP = true;
            }
        }
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

        AddExperience(pedido.experiencia);
        Debug.Log("Pedido entregado correctamente.");
    }
}
