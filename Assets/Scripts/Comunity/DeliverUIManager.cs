using UnityEngine;

public class DeliverUIManager : MonoBehaviour
{
    [Header("Referencias")]
    public DeliverManager deliverManager;
    public GameObject pedidoUIPrefab;  // prefab del pedido
    public Transform container;        // contenedor en el Canvas

    private void Start()
    {
        Debug.Log("Paso?");
        MostrarPedidos();
    }

    public void MostrarPedidos()
    {
        // Limpia pedidos previos
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Instanciar los 3 pedidos actuales
        var pedidos = deliverManager.GetPedidosActuales();
        int nivel = deliverManager.nivelActual;

        foreach (var pedido in pedidos)
        {
            Debug.Log($"Instanciando pedido en UI -> Zanahorias:{pedido.zanahorias} Papas:{pedido.papas}");
            var pedidoUIObj = Instantiate(pedidoUIPrefab, container);

            RectTransform rt = pedidoUIObj.GetComponent<RectTransform>();

            var pedidoUI = pedidoUIObj.GetComponent<PedidoUI>();
            pedidoUI.SetPedido(pedido, nivel);
        }
    }
}
