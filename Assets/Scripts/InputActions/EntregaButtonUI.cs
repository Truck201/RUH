using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class EntregaButtonUI : MonoBehaviour
{
    [Header("Referencias")]
    public Button entregaButton;
    public TMP_Text inputText; // Texto que muestra la tecla / botón
    public float interactRadius = 2f;

    private GameObject player;
    private PedidoUI currentPedidoUI;

    private void Awake()
    {
        if (entregaButton != null)
        {
            entregaButton.onClick.AddListener(OnButtonPressed);
            entregaButton.gameObject.SetActive(false);
        }

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (player == null || currentPedidoUI == null) return;

        // 🔹 Mostrar u ocultar botón según distancia
        float dist = Vector3.Distance(player.transform.position, transform.position);
        entregaButton.gameObject.SetActive(dist <= interactRadius);

        // 🔹 Actualizar texto del botón según el dispositivo
        UpdateInputText();

        // 🔹 También se puede disparar por input directamente
        if (dist <= interactRadius && IsEntregarPressed())
        {
            OnButtonPressed();
        }
    }

    private void UpdateInputText()
    {
        if (Keyboard.current.anyKey.isPressed)
        {
            inputText.text = InputContextManager.Instance.Inputs.Gameplay.Entregar.controls[0].displayName;
        }
        else if (Gamepad.current != null)
        {
            inputText.text = InputContextManager.Instance.Inputs.Gameplay.Entregar.controls[0].displayName;
        }
        else
        {
            inputText.text = "?";
        }
    }

    private bool IsEntregarPressed()
    {
        var inputAction = InputContextManager.Instance.Inputs.Gameplay.Entregar;
        return inputAction.WasPerformedThisFrame();
    }

    public void SetPedidoUI(PedidoUI pedidoUI)
    {
        currentPedidoUI = pedidoUI;
    }

    private void OnButtonPressed()
    {
        if (currentPedidoUI != null)
        {
            currentPedidoUI.TryEntregarPedido();
        }
    }
}
