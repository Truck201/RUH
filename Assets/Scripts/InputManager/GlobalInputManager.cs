using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class GlobalInputManager : MonoBehaviour
{
    public static GlobalInputManager Instance;
    private PlayerInputs inputs => InputContextManager.Instance.Inputs;

    [Header("Gamepads asignados")]
    public bool IsUsingGamepad { get; private set; }
    public Gamepad AssignedGamepad { get; private set; }

    // Gameplay inputs
    public Vector2 Move { get; private set; }
    public bool Run { get; private set; }
    public bool Attack { get; private set; }
    public bool Deliver { get; private set; }
    public bool Vaccum { get; private set; }
    public bool Pause { get; private set; }
    public bool Scope { get; private set; }

    // Pause menu
    public Vector2 PauseMove { get; private set; }
    public bool PauseSelect { get; private set; }
    public bool PauseQuit { get; private set; }

    // Dialogue
    public bool DialoguePass { get; private set; }


    private Vector2 lastMove = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DetectGamepad();
            SubscribeInputs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DetectGamepad()
    {
        var pads = Gamepad.all;
        AssignedGamepad = pads.Count > 0 ? pads[0] : null;
        IsUsingGamepad = AssignedGamepad != null;
        Debug.Log($"🎮 Gamepad detectado: {(AssignedGamepad != null ? AssignedGamepad.displayName : "Ninguno")}");
    }

    private void SubscribeInputs()
    {
        var player = inputs;

        // Gameplay
        player.Gameplay.Move.performed += ctx => { Move = ctx.ReadValue<Vector2>(); DetectDevice(ctx); };
        player.Gameplay.Move.canceled += ctx => Move = Vector2.zero;

        player.Gameplay.Run.performed += ctx => { Run = true; DetectDevice(ctx); };
        player.Gameplay.Run.canceled += _ => Run = false;

        player.Gameplay.Attack.performed += ctx => { Attack = true; DetectDevice(ctx); };
        player.Gameplay.Attack.canceled += _ => Attack = false;

        player.Gameplay.Deliver.performed += ctx => { Deliver = true; DetectDevice(ctx); };
        player.Gameplay.Deliver.canceled += _ => Deliver = false;

        player.Gameplay.Vaccum.performed += ctx => { Vaccum = true; DetectDevice(ctx); };
        player.Gameplay.Vaccum.canceled += _ => Vaccum = false;

        player.Gameplay.Pause.performed += ctx => { Pause = true; DetectDevice(ctx); };
        player.Gameplay.Scope.performed += ctx => { Scope = true; DetectDevice(ctx); };

        // Pause Menu
        player.PauseMenu.Move.performed += ctx => { PauseMove = ctx.ReadValue<Vector2>(); DetectDevice(ctx); };
        player.PauseMenu.Move.canceled += _ => PauseMove = Vector2.zero;

        player.PauseMenu.Select.performed += ctx => { PauseSelect = true; DetectDevice(ctx); };
        player.PauseMenu.Quit.performed += ctx => { PauseQuit = true; DetectDevice(ctx); };

        // Dialogue
        player.Dialogue.Pass.performed += ctx => { DialoguePass = true; DetectDevice(ctx); };
    }

    private void DetectDevice(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Gamepad gp)
        {
            if (AssignedGamepad != gp)
            {
                AssignedGamepad = gp;
                IsUsingGamepad = true;
                Debug.Log($"🎮 Nuevo gamepad activo: {gp.displayName}");
            }
        }
        else if (ctx.control.device is Keyboard)
        {
            IsUsingGamepad = false;
        }
    }

    private void LateUpdate()
    {
        // limpiar flags one-frame
        Attack = Deliver = Scope = Pause = PauseQuit = PauseSelect = DialoguePass = false;
    }

    public bool AttackPressed() => Attack;
    public bool DeliverPressed() => Deliver;
    public bool VaccumPressed() => Vaccum;
    public bool PausePressed() => Pause;
    public bool ScopePressed() => Scope;

    public bool HaveGamepad() => IsUsingGamepad;

    public string GetCurrentDeliverBinding()
    {
        if (inputs == null || inputs.Gameplay.Deliver == null)
        {
            Debug.LogWarning("⚠️ Inputs no inicializados al intentar obtener el binding de Deliver.");
            return string.Empty;
        }

        InputBinding binding;

        if (IsUsingGamepad)
        {
            binding = inputs.Gameplay.Deliver.bindings
                .FirstOrDefault(b => b.groups != null && b.groups.Contains("Gamepad"));
        }
        else
        {
            binding = inputs.Gameplay.Deliver.bindings
                .FirstOrDefault(b => b.groups != null && b.groups.Contains("Keyboard"));
        }

        if (binding == null)
        {
            Debug.LogWarning("⚠️ No se encontró binding válido para Deliver.");
            return string.Empty;
        }

        try
        {
            return binding.ToDisplayString();
        }
        catch
        {
            Debug.LogWarning("⚠️ Error al convertir binding a display string.");
            return string.Empty;
        }
    }

}
