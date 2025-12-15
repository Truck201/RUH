using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Gameplay,
    Paused,
    Tutorial,
    Cutscene,
    Dialogue
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public GameState CurrentState { get; private set; } = GameState.Gameplay;

    public event System.Action<GameState> OnGameStateChanged;

    public bool introCompletada = false;

    private void Awake()
    {
        // Singleton persistente
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetState(GameState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;
        OnGameStateChanged?.Invoke(CurrentState);
    }
}
