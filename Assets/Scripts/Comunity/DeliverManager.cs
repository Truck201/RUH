using UnityEngine;
using System.Collections.Generic;

public class DeliverManager : MonoBehaviour
{
    public static DeliverManager Instance { get; private set; }

    [Header("Referencia al ScriptableObject")]
    public ComunityDeliver deliverData;

    [Header("Nivel actual del jugador (0-5)")]
    public int nivelActual;
    [SerializeField] private PlayerStats playerStats;

    private List<ComunityDeliver.DeliverLevel> pedidosCompletados = new List<ComunityDeliver.DeliverLevel>();
    private List<ComunityDeliver.DeliverLevel> pedidosActuales = new List<ComunityDeliver.DeliverLevel>();

    private void Awake()
    {
        if (DeliverManager.Instance == null)
        {
            DeliverManager.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        nivelActual = playerStats.nivelActual;
        SetPedidosPorNivel(nivelActual);
    }

    public void SetPedidosPorNivel(int nivel)
    {
        pedidosActuales.Clear();

        if (nivel >= 0 && nivel < deliverData.niveles.Length)
        {
            var opciones = deliverData.niveles[nivel].pedidos;


            if (opciones.Length < 3)
            {
                Debug.LogWarning("No hay suficientes pedidos para elegir 3.");
                return;
            }

            // Elegir 3 pedidos random distintos
            List<int> indicesUsados = new List<int>();
            while (pedidosActuales.Count < 3)
            {
                int randomIndex = Random.Range(0, opciones.Length);
                if (!indicesUsados.Contains(randomIndex))
                {
                    indicesUsados.Add(randomIndex); 
                    pedidosActuales.Add(opciones[randomIndex]);
                }
            }

            // Mostrar en consola
            for (int i = 0; i < pedidosActuales.Count; i++)
            {
                var pedido = pedidosActuales[i];
                Debug.Log($"Pedido {i + 1}: Zanahorias {pedido.zanahorias}, Papas {pedido.papas}, Cebollas {pedido.cebollas}, Exp {pedido.experiencia}");
            }
        }
    }

    public void CompletarPedido(ComunityDeliver.DeliverLevel pedido)
    {
        PlayerStats.Instance.EntregarPedido(pedido);

        // ✅ marcar pedido como completado
        if (!pedidosCompletados.Contains(pedido))
            pedidosCompletados.Add(pedido);

        Debug.Log($"Pedido completado ({pedidosCompletados.Count}/{pedidosActuales.Count})");

        // ✅ si todos los pedidos del nivel actual están entregados → tomar nuevos
        if (pedidosCompletados.Count >= pedidosActuales.Count)
        {
            Debug.Log($"🎉 Todos los pedidos del nivel {nivelActual} completados. Generando nuevos...");
            pedidosCompletados.Clear();
            SetPedidosPorNivel(nivelActual); // vuelve a elegir 3 pedidos del mismo nivel

            // 🔹 actualizar la UI
            var uiManager = FindFirstObjectByType<DeliverUIManager>();
            if (uiManager != null)
            {
                uiManager.MostrarPedidos();
            }
        }
    }

    public void ClearCompletedPedidos()
    {
        pedidosCompletados.Clear();
    }

    public List<ComunityDeliver.DeliverLevel> GetPedidosActuales()
    {
        return pedidosActuales;
    }
}
