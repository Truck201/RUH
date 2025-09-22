using UnityEngine;
using System.Collections;

public static class CoroutineUtils
{
    public static IEnumerator WaitForSecondsPaused(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            if (!GamePauseManager.Instance.IsPaused)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
    }
}

// yield return CoroutineUtils.WaitForSecondsPaused(3f);  Ejemplo de uso