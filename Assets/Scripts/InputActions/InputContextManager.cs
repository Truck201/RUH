using UnityEngine;
using UnityEngine.InputSystem;

public class InputContextManager : MonoBehaviour
{
    public static InputContextManager Instance { get; private set; }

    public PlayerInputs Inputs { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Inputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager no está inicializado antes de InputContextManager");
            return;
        }

        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        OnGameStateChanged(GameStateManager.Instance.CurrentState); // Forzar estado inicial
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
        // Primero desactivamos todos
        Inputs.Gameplay.Disable();
        Inputs.PauseMenu.Disable();
        Inputs.Inventory.Disable();

        // Activamos según el estado
        switch (newState)
        {
            case GameState.Gameplay:
                Inputs.Gameplay.Enable();
                Debug.Log("Gameplay");
                break;
            case GameState.Paused:
                Inputs.PauseMenu.Enable();
                Debug.Log("Paused");
                break;
            case GameState.Inventory:
                Inputs.Inventory.Enable();
                Debug.Log("Inventory");
                break;
        }
    }
}
