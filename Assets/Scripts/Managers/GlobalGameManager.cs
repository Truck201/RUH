using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private GameObject generalCanvas;

    [Header("First Selected Objects")]
    [SerializeField] private GameObject firstPauseSelected;
    [SerializeField] private GameObject firstInventorySelected;

    [Header("Scene Filtering")]
    [SerializeField] private string[] ignoredScenes;
    // Ej: {"MainMenu", "CutsceneScene"}

    private void Awake()
    {
        // Singleton persistente
        // if (Instance == null)
        // {
        //     Instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

        //pauseCanvas = GameObject.Find("PauseCanvas");
        //firstPauseSelected =   GameObject.Find("ResumePauseText");
        pauseCanvas.SetActive(false);
    }

    void Update()
    {
        var pauseInput = GlobalInputManager.Instance.Pause;
        var quitInput = GlobalInputManager.Instance.PauseQuit;

        if (pauseInput || quitInput)
        {
            GameStateManager.Instance.SetState(
                GameStateManager.Instance.CurrentState == GameState.Gameplay
                ? GameState.Paused
                : GameState.Gameplay
            );

            GamePauseManager.Instance.TogglePause();
            Debug.Log("Confirm Pause");
        }
    }

    private void OnEnable()
    {
        GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si la escena est� en la lista ignorada, ocultamos todo
        if (IsIgnoredScene(scene.name))
        {
            if (pauseCanvas) pauseCanvas.SetActive(false);
            if (inventoryCanvas) inventoryCanvas.SetActive(false);
            if (generalCanvas) generalCanvas.SetActive(true);
        }

        // pauseCanvas = GameObject.Find("PauseCanvas");
        // firstPauseSelected = GameObject.Find("ResumePauseText");
        // pauseCanvas.SetActive(false);
    }

    private bool IsIgnoredScene(string sceneName)
    {
        foreach (var ignored in ignoredScenes)
        {
            if (sceneName == ignored) return true;
        }
        return false;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        // Si estamos en una escena ignorada, no hacemos nada
        if (IsIgnoredScene(SceneManager.GetActiveScene().name))
            return;

        // Activar/Desactivar seg�n el estado
        switch (newState)
        {
            case GameState.Paused:
                if (pauseCanvas) pauseCanvas.SetActive(true);
                if (generalCanvas) generalCanvas.SetActive(false);
                if (inventoryCanvas) inventoryCanvas.SetActive(false);
                SetFirstSelected(firstPauseSelected);
                break;

            case GameState.Inventory:
                if (pauseCanvas) pauseCanvas.SetActive(false);
                if (generalCanvas) generalCanvas.SetActive(true);
                if (inventoryCanvas) inventoryCanvas.SetActive(true);
                SetFirstSelected(firstInventorySelected);
                break;

            default:
                if (pauseCanvas) pauseCanvas.SetActive(false);
                if (generalCanvas) generalCanvas.SetActive(true);
                if (inventoryCanvas) inventoryCanvas.SetActive(false);
                SetFirstSelected(null);
                break;
        }
    }

    private void SetFirstSelected(GameObject obj)
    {
        if (EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(null); // Limpia el focus
        if (obj != null)
        {
            EventSystem.current.SetSelectedGameObject(obj);
        }
    }
}

