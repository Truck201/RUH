using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Jugar()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.introCompletada)
        {
            // Ya vio la intro → Ir directo a Farm
            SceneManager.LoadScene("Farm");
        }
        else
        {
            // Primera vez → Mostrar intro
            SceneManager.LoadScene("CartoonedStart");
        }
    }
}
