using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputContextManager : MonoBehaviour
{
    public static InputContextManager Instance { get; private set; }
    public static event Action<bool> OnDeviceChanged;

    public PlayerInputs Inputs { get; private set; }

    private bool isUsingGamepad;


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

    private void Start()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDestroy()
    {
        InputSystem.onEvent -= OnInputEvent;
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

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device == null) return;

        bool newState = device is Gamepad;

        // Solo disparamos evento si realmente cambia el tipo de dispositivo
        if (newState != isUsingGamepad)
        {
            isUsingGamepad = newState;
            OnDeviceChanged?.Invoke(isUsingGamepad);
            Debug.Log($"🔄 Dispositivo cambiado: {(isUsingGamepad ? "Gamepad" : "Teclado")}");
        }
    }

    public bool IsUsingGamepad()
    {
        return isUsingGamepad;
    }

    // 🔹 Helper genérico: obtiene el display string del binding
    public string GetBindingDisplayString(InputAction action)
    {
        if (action == null) return "?";

        var bindings = action.bindings.ToList();

        if (Gamepad.current != null)
        {
            int bindingIndex = bindings.FindIndex(b => !string.IsNullOrEmpty(b.groups) && b.groups.Contains("Gamepad"));
            if (bindingIndex >= 0)
                return action.GetBindingDisplayString(bindingIndex);
            return "X";
        }
        else
        {
            int bindingIndex = bindings.FindIndex(b => !string.IsNullOrEmpty(b.groups) && b.groups.Contains("Keyboard"));
            if (bindingIndex >= 0)
                return action.GetBindingDisplayString(bindingIndex);
            return "E";
        }
    }
}
