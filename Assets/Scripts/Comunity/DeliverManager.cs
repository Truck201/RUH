using UnityEngine;
using System.Collections.Generic;

public class DeliverManager : MonoBehaviour
{
    [Header("Referencia al ScriptableObject")]
    public ComunityDeliver deliverData;

    [Header("Nivel actual del jugador (0-5)")]
    public int nivelActual;
    [SerializeField] private PlayerStats playerStats;

    private List<ComunityDeliver.DeliverLevel> pedidosActuales = new List<ComunityDeliver.DeliverLevel>();

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
    }


    public List<ComunityDeliver.DeliverLevel> GetPedidosActuales()
    {
        return pedidosActuales;
    }
}
