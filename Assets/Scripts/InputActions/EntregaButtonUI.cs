using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class EntregaButtonUI : MonoBehaviour
{
    [Header("Referencias")]
    public Button entregaButton;
    public TMP_Text inputText; // Texto que muestra la tecla / botón
    public float interactRadius = 2f;

    private GameObject player;

    [SerializeField] PedidoUI currentPedidoUI;
    private DeliverManager deliverManager;

    private PlayerInputs playerInputs;
    private bool isProcessing = false;

    private void Awake()
    {
        if (entregaButton == null)
            entregaButton = GetComponentInChildren<Button>();

        if (entregaButton)
        {
            entregaButton.onClick.AddListener(OnButtonPressed);
            entregaButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("EntregaButtonUI: no se encontró Button en hijos ni fue asignado.");
        }

        playerInputs = new PlayerInputs();
        playerInputs.Gameplay.Entregar.performed += ctx => OnButtonPressed();

        player = GameObject.FindGameObjectWithTag("Player");
        deliverManager = Object.FindFirstObjectByType<DeliverManager>();
    }

    private void Update()
    {
        if (currentPedidoUI == null)
        {
            if (entregaButton) entregaButton.gameObject.SetActive(false);
            return;
        }

        // 🔹 Mostrar u ocultar botón según distancia
        float dist = Vector3.Distance(player.transform.position, currentPedidoUI.transform.position);
        bool inRange = dist <= interactRadius;

        //Debug.Log("UpdateButton");

        bool puedeEntregar = currentPedidoUI != null &&
                     currentPedidoUI.GetCurrentPedido() != null &&
                     PlayerStats.Instance.PuedeCompletarPedido(currentPedidoUI.GetCurrentPedido());

        if (entregaButton)
            entregaButton.gameObject.SetActive(inRange && puedeEntregar);

        if (!inRange && puedeEntregar) return;

        //Debug.Log($"|| CONDITION || {!inRange} y {puedeEntregar} || Más características -> {dist} & {inRange}");

        UpdateInputText();

        if (IsEntregarPressed())
        {
            OnButtonPressed();
        }
    }

    private void UpdateInputText()
    {
        inputText.text = InputContextManager.Instance.GetBindingDisplayString(
            InputContextManager.Instance.Inputs.Gameplay.Entregar
        );
    }


    private bool IsEntregarPressed()
    {
        var inputAction = InputContextManager.Instance.Inputs.Gameplay.Entregar;
        //Debug.Log("Pressed Entregar");
        return inputAction.WasPerformedThisFrame();
    }

    public void SetPedidoUI(PedidoUI pedidoUI)
    {
        currentPedidoUI = pedidoUI;
    }

    private void OnButtonPressed()
    {
        if (isProcessing) return;
        isProcessing = true;

        if (currentPedidoUI == null)
        {
            Debug.LogWarning("EntregaButtonUI: currentPedidoUI null en OnButtonPressed.");
            isProcessing = false;
            return;
        }

        var pedido = currentPedidoUI.GetCurrentPedido();
        if (pedido == null)
        {
            Debug.LogWarning("EntregaButtonUI: No hay pedido actual en PedidoUI.");
            isProcessing = false;
            return;
        }

        // 🔹 Chequear y entregar usando DeliverManager
        if (PlayerStats.Instance.PuedeCompletarPedido(pedido))
        {
            deliverManager.CompletarPedido(pedido);

            currentPedidoUI.OnPedidoDelivered();

            if (entregaButton) entregaButton.gameObject.SetActive(false);

            Debug.Log("✅ Pedido entregado con DeliverManager!");
        }
        else
        {
            Debug.LogWarning("No tienes suficientes ingredientes para este pedido.");
        }

        isProcessing = false;
    }
}
