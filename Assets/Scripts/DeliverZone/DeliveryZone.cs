using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [Header("Radio de interacci�n")]
    public float radius = 3f;

    [Header("Jugador")]
    public Transform player;

    [Header("UI del bot�n")]
    public GameObject entregarButton;

    private ComunityDeliver.DeliverLevel pedidoActual;

    void Start()
    {
        entregarButton.SetActive(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        // Mostrar / ocultar bot�n seg�n radio
        if (dist <= radius)
        {
            entregarButton.SetActive(true);
        }
        else
        {
            entregarButton.SetActive(false);
        }
    }

    // Asigna el pedido que representa esta zona
    public void SetPedido(ComunityDeliver.DeliverLevel pedido)
    {
        pedidoActual = pedido;
    }

    // Llamado por el bot�n
    public void IntentarEntregar()
    {
        if (pedidoActual == null) return;

        if (PlayerStats.Instance.PuedeCompletarPedido(pedidoActual))
        {
            DeliverManager dm = Object.FindFirstObjectByType<DeliverManager>();
            dm.CompletarPedido(pedidoActual);
            entregarButton.SetActive(false);
        }
        else
        {
            Debug.Log("No tienes los ingredientes necesarios para este pedido.");
        }
    }
}
