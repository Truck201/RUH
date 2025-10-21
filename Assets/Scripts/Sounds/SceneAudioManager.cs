using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAudioManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SoundController.Instance == null) return;

        AudioClip sceneMusic = SoundController.Instance.GetSceneMusic(scene.name);

        if (sceneMusic != null)
            SoundController.Instance.PlayMusic(sceneMusic);
    }
}
