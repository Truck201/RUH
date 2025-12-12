using UnityEngine;

public class OutGame : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("CHAU");
    }
}