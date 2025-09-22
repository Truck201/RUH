using UnityEngine;

public class PauseInput : MonoBehaviour
{
    private void OnEnable()
    {
        var inputs = InputContextManager.Instance.Inputs;

        inputs.Gameplay.Pause.performed += ctx =>
        {
            GameStateManager.Instance.SetState(
                GameStateManager.Instance.CurrentState == GameState.Gameplay
                ? GameState.Paused
                : GameState.Gameplay
            );

            GamePauseManager.Instance.TogglePause();
            Debug.Log("Confirm Pause");
        };

        inputs.Gameplay.Inventory.performed += ctx =>
        {
            GameStateManager.Instance.SetState(
                GameStateManager.Instance.CurrentState == GameState.Gameplay
                ? GameState.Inventory
                : GameState.Gameplay
            );

            GamePauseManager.Instance.TogglePause();
            Debug.Log("Confirm Pause");
        };

        inputs.PauseMenu.Quit.performed += ctx =>
        {
            GameStateManager.Instance.SetState(GameState.Gameplay);
            GamePauseManager.Instance.TogglePause();
        };

        inputs.Inventory.Quit.performed += ctx =>
        {
            GameStateManager.Instance.SetState(GameState.Gameplay);
            GamePauseManager.Instance.TogglePause();
        };
    }
}
