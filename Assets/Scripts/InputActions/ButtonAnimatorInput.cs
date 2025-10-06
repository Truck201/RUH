using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimatorInput : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Image buttonImage;

    [Header("Sprites opcionales")]
    public Sprite keyboardSprite;
    public Sprite gamepadSprite;

    private void Start()
    {
        // Configurar animación inicial
        bool isGamepad = InputContextManager.Instance.IsUsingGamepad();
        UpdateButtonVisual(isGamepad);

        // Escuchar cambios de dispositivo
        InputContextManager.OnDeviceChanged += UpdateButtonVisual;
    }

    private void OnDestroy()
    {
        InputContextManager.OnDeviceChanged -= UpdateButtonVisual;
    }

    private void UpdateButtonVisual(bool usingGamepad)
    {
        if (animator)
        {
            animator.SetBool("UsingGamepad", usingGamepad);
        }

        if (buttonImage)
        {
            buttonImage.sprite = usingGamepad ? gamepadSprite : keyboardSprite;
        }
    }
}
