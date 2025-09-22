using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    [Header("Radio de interacción")]
    public float radius = 3f;

    [Header("Jugador")]
    public Transform player;

    [Header("UI del botón")]
    public GameObject entregarButton;

    private ComunityDeliver.DeliverLevel pedidoActual;

    void Start()
    {
        entregarButton.SetActive(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        // Mostrar / ocultar botón según radio
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

    // Llamado por el botón
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
