using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public string sceneName;
    public float delay = 0f;

    private bool hasChanged = false;

    public void ChangeScene()
    {
        if (hasChanged || string.IsNullOrEmpty(sceneName)) return;

        Debug.Log("Cambiando escena...");
        hasChanged = true;

        if (delay > 0f)
            StartCoroutine(DelayedSceneLoad());
        else
            LoadScene();
    }

    private IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(delay);
        LoadScene();
    }

    private void LoadScene()
    {
        Debug.Log($"Change Scene {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
