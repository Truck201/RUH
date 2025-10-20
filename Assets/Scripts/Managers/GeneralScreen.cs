using UnityEngine;

public class GeneralScreen : MonoBehaviour
{
    public static GeneralScreen Instance;

    private void Awake()
    {
        // Singleton persistente
        if (GeneralScreen.Instance == null)
        {
            GeneralScreen.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
