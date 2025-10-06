using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PedidoUI : MonoBehaviour
{
    [Header("Contenedores")]
    public GameObject zanahoriaPanel;
    public GameObject papaPanel;
    public GameObject cebollaPanel;

    [Header("Referencias UI")]
    public TMP_Text zanahoriasText;
    public TMP_Text papasText;
    public TMP_Text cebollasText;
    public TMP_Text experienciaText;

    [Header("Interacción")]
    public Button entregarButton;
    public float interactRadius = 4f; // radio de proximidad
    private GameObject player;

    private ComunityDeliver.DeliverLevel currentPedido;
    private int nivel;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        var entregaUI = GetComponentInChildren<EntregaButtonUI>();
        if (entregaUI != null)
        {
            entregaUI.SetPedidoUI(this);
        }
    }

    private void Update()
    {
        //if (player == null || currentPedido == null) return;

        // 🔹 Verificamos proximidad
        float dist = Vector3.Distance(player.transform.position, transform.position);
        entregarButton.gameObject.SetActive(dist <= interactRadius);
        // Debug.Log($"Distancia -> {dist} || Radio interact -> {interactRadius}");
    }
    public void SetPedido(ComunityDeliver.DeliverLevel pedido, int nivel)
    {
        currentPedido = pedido;
        this.nivel = nivel;

        // Zanahorias
        if (pedido.zanahorias > 0)
        {
            zanahoriaPanel.SetActive(true);
            zanahoriasText.text = $"{pedido.zanahorias}";
        }
        else
        {
            zanahoriaPanel.SetActive(false);
        }

        // Papas
        if (pedido.papas > 0)
        {
            papaPanel.SetActive(true);
            papasText.text = $"{pedido.papas}";
        }
        else
        {
            papaPanel.SetActive(false);
        }

        // Cebollas
        if (pedido.cebollas > 0)
        {
            cebollaPanel.SetActive(true);
            cebollasText.text = $"{pedido.cebollas}";
        }
        else
        {
            cebollaPanel.SetActive(false);
        }

        // Experiencia
        experienciaText.text = $"+{pedido.experiencia} XP";

        // Colorear XP según nivel
        switch (nivel)
        {
            case 0: experienciaText.color = Color.gray; break;
            case 1: experienciaText.color = Color.green; break;
            case 2: experienciaText.color = Color.yellow; break;
            case 3: experienciaText.color = Color.cyan; break;
            case 4: experienciaText.color = new Color(1f, 0.5f, 0f); break;
            case 5: experienciaText.color = Color.red; break;
        }
    }

    public bool TryEntregarPedido()
    {
        if (currentPedido == null)
        {
            Debug.LogWarning("No hay pedido asignado a este PedidoUI.");
            return false;
        }

        // 🔹 Chequeamos si hay suficientes ingredientes
        if (PlayerStats.Instance.PuedeCompletarPedido(currentPedido))
        {
            PlayerStats.Instance.EntregarPedido(currentPedido);
            currentPedido = null;

            if (entregarButton)
                entregarButton.gameObject.SetActive(false);

            Debug.Log("Pedido entregado con éxito!");
            return true;
        }
        else
        {
            Debug.LogWarning("No tienes suficientes ingredientes para este pedido.");
            return false;
        }
    }

    public void OnPedidoDelivered()
    {
        // ejemplo: desactivar el objeto del pedido
        gameObject.SetActive(false);
    }

    // getter para que EntregaButtonUI pueda consultar rápidamente
    public ComunityDeliver.DeliverLevel GetCurrentPedido() => currentPedido;
}
