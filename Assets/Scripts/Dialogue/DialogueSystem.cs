using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class DialogueSystem : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Image npcPortrait;

    private SpriteRenderer sprite;
    private Animator animator;    

    [Header("Secuencias de diálogo")]
    public DialogueLine[] dialogueLinesSet1;
    public DialogueLine[] dialogueLinesSet2;
    public DialogueLine[] dialogueLinesSet3;
    public DialogueLine[] defaultDialogueLines;

    [Header("Configuración general")]
    public float textDuration = 4f;

    [Header("Cámara")]
    private SmoothCameraFollow camFollow;
    private Transform playerTransform;

    private bool inRange;
    private bool dialogueActive;
    private int dialoguePhase = 0; // controla en qué secuencia va el jugador

    [Header("Interacción")]
    public GameObject interactIcon;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        camFollow = Camera.main.GetComponent<SmoothCameraFollow>();
        if (camFollow == null)
            Debug.LogError("No se encontró SmoothCameraFollow en la cámara principal");

        dialoguePanel.SetActive(false);
        interactIcon.SetActive(false);
    }

    private void Update()
    {
        if (GlobalInputManager.Instance == null)
            return; 

        if (inRange && !dialogueActive && GlobalInputManager.Instance.DeliverPressed())
        {
            DialogueLine[] selectedLines = GetDialogueSet();
            StartCoroutine(StartDialogueSequence(selectedLines));
        }
    }

    private DialogueLine[] GetDialogueSet()
    {
        switch (dialoguePhase)
        {
            case 0: return dialogueLinesSet1;
            case 1: return dialogueLinesSet2;
            case 2: return dialogueLinesSet3;
            default: return defaultDialogueLines;
        }
    }

    private IEnumerator StartDialogueSequence(DialogueLine[] dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning("No hay diálogos en esta secuencia.");
            yield break;
        }

        dialogueActive = true;
        interactIcon.SetActive(false);
        GameStateManager.Instance.SetState(GameState.Dialogue);

        playerTransform = camFollow.target;

        if (animator != null)
            animator.Play("Dialogue");

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            DialogueLine line = dialogueLines[i];

            // 🔹 Enfocar cámara
            if (line.cameraFocus != null)
                camFollow.target = line.cameraFocus;

            // 🔹 Mostrar retrato
            if (line.npcFace != null)
                npcPortrait.sprite = line.npcFace;


            // 🔹 Ocultar los de la línea anterior
            if (i > 0)
            {
                DialogueLine prevLine = dialogueLines[i - 1];
                if (prevLine.haveGameObjects && prevLine.tutorialObjects != null && !prevLine.activeInGame)
                {
                    foreach (var obj in prevLine.tutorialObjects)
                        if (obj != null) obj.SetActive(false);
                }
            }

            // 🔹 Mostrar objetos de tutorial si los hay
            if (line.haveGameObjects && line.tutorialObjects != null)
            {
                foreach (var obj in line.tutorialObjects)
                    if (obj != null) obj.SetActive(true);
            }

            // 🔹 Mostrar texto
            dialoguePanel.SetActive(true);
            yield return StartCoroutine(TypeText(line.text));
            yield return new WaitForSeconds(textDuration);
        }

        // 🔹 Apagar tutorial objects del último diálogo
        DialogueLine lastLine = dialogueLines[dialogueLines.Length - 1];
        if (lastLine.haveGameObjects && lastLine.tutorialObjects != null && !lastLine.activeInGame)
        {
            foreach (var obj in lastLine.tutorialObjects)
                if (obj != null) obj.SetActive(false);
        }

        // 🔹 Volver al jugador
        camFollow.target = playerTransform;
        dialoguePanel.SetActive(false);
        dialogueActive = false;

        // 🔹 Pasar a la siguiente secuencia
        if (dialoguePhase < 3)
            dialoguePhase++;

        if (animator != null)
            animator.Play("idle");

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
            interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
            interactIcon.SetActive(false);
        }
    }
}
