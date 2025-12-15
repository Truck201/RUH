using UnityEngine;

public class DummyScript : MonoBehaviour
{
    private bool inRange;
    private bool isInAttackMode;

    [Header("Interacción")]
    public GameObject interactIcon;

    [Header("Activar Dummy")]
    public bool activeDummy = false;

    [SerializeField] UIButtonPrompt buttonAction;

    void Start()
    {
        interactIcon.SetActive(false);

        // Estado inicial
        buttonAction.SwitchButtonActionType(ButtonActionType.Scope);
        isInAttackMode = false;
    }

    void Update()
    {
        if (!inRange) return;

        if (!activeDummy && PlayerStats.Instance.nivelActual == 2)
        {
            activeDummy = true;
        }

        // Cuando presiona Scope → cambia a Attack
        if (!isInAttackMode && GlobalInputManager.Instance.ScopePressed() && activeDummy)
        {
            isInAttackMode = true;
            buttonAction.SwitchButtonActionType(ButtonActionType.Attack);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsDummyActive()) return;
        if (!collision.CompareTag("Player")) return;

        inRange = true;
        interactIcon.SetActive(true);

        // Siempre que entra en rango, empieza en Scope
        isInAttackMode = false;
        ResetToScope();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        inRange = false;
        interactIcon.SetActive(false);

        // Al salir de rango vuelve a Scope
        isInAttackMode = false;
        ResetToScope();
    }

    private bool IsDummyActive()
    {
        return PlayerStats.Instance != null &&
               PlayerStats.Instance.nivelActual >= 2;
    }

    private void ResetToScope()
    {
        isInAttackMode = false;
        buttonAction.SwitchButtonActionType(ButtonActionType.Scope);
    }
}
