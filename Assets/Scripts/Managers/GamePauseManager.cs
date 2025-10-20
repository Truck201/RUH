using UnityEngine;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

public class GamePauseManager : MonoBehaviour
{
    public static GamePauseManager Instance { get; private set; }

    public bool IsPaused { get; private set; }
    public event Action<bool> OnPauseStateChanged;

    private void Awake()
    {
        // Singleton persistente
        if (GamePauseManager.Instance == null)
        {
            GamePauseManager.Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPause(bool pause)
    {
        if (IsPaused == pause) return;

        IsPaused = pause;
        OnPauseStateChanged?.Invoke(IsPaused);
    }

    public void TogglePause()
    {
        SetPause(!IsPaused);
    }
}

// if (GamePauseManager.Instance.IsPaused) return;   Ejemplo de uso
