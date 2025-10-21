using UnityEngine;
using UnityEngine.InputSystem;

public enum ButtonActionType
{
    MoveUp, MoveDown, MoveLeft, MoveRight,
    Deliver, PassDialogue,
    Attack, Run, Scope, Vaccum
}

public class UIButtonPrompt : MonoBehaviour
{
    [Header("¿Qué acción representa este botón?")]
    public ButtonActionType actionType;

    [Header("Animator del botón")]
    public Animator animator;

    [Header("Override Keyboard")]
    public AnimatorOverrideController W_button;
    public AnimatorOverrideController A_button;
    public AnimatorOverrideController S_button;
    public AnimatorOverrideController D_button;
    public AnimatorOverrideController E_button;
    public AnimatorOverrideController Q_button;
    public AnimatorOverrideController Shift_button;
    public AnimatorOverrideController MouseLeft_button;
    public AnimatorOverrideController MouseRight_button;

    [Header("Override Gamepad")]
    public AnimatorOverrideController X_button;
    public AnimatorOverrideController O_button;
    public AnimatorOverrideController L1_button;
    public AnimatorOverrideController R1_button;
    public AnimatorOverrideController Pad_button;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdatePromptVisual();
    }

    void UpdatePromptVisual()
    {
        if (GlobalInputManager.Instance == null || animator == null) return;

        bool usingGamepad = GlobalInputManager.Instance.HaveGamepad();
        AnimatorOverrideController selectedController = null;

        switch (actionType)
        {
            case ButtonActionType.Deliver:
            case ButtonActionType.PassDialogue:
                selectedController = usingGamepad ? X_button : E_button;
                break;

            case ButtonActionType.Attack:
                selectedController = usingGamepad ? X_button : MouseLeft_button;
                break;

            case ButtonActionType.Run:
                selectedController = usingGamepad ? R1_button : Shift_button;
                break;

            case ButtonActionType.Scope:
                selectedController = usingGamepad ? L1_button : Q_button;
                break;

            case ButtonActionType.Vaccum:
                selectedController = usingGamepad ? O_button : MouseRight_button;
                break;

            case ButtonActionType.MoveUp:
                selectedController = usingGamepad ? Pad_button : W_button;
                break;
            case ButtonActionType.MoveDown:
                selectedController = usingGamepad ? Pad_button : S_button;
                break;
            case ButtonActionType.MoveLeft:
                selectedController = usingGamepad ? Pad_button : A_button;
                break;
            case ButtonActionType.MoveRight:
                selectedController = usingGamepad ? Pad_button : D_button;
                break;
        }

        if (selectedController != null && animator.runtimeAnimatorController != selectedController)
        {
            animator.runtimeAnimatorController = selectedController;
        }
    }
}
