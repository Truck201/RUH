using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionIconAppear : MonoBehaviour
{
    [Header("Referencias a los botones del menú")]
    public GameObject buttonPlay;
    public GameObject buttonOptions;
    public GameObject buttonExit;

    [Header("Iconos a mostrar")]
    public GameObject iconPlay;
    public GameObject iconOptions;
    public GameObject iconExit;

    public SceneChanger sceneChanger;

    private void Update()
    {
        // 🔹 Obtener el objeto actualmente seleccionado por el EventSystem
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;

        // 🔹 Cambiar visibilidad de los iconos según selección
        if (iconPlay)
        {
            iconPlay.SetActive(selected == buttonPlay);
        }

        
        if (iconOptions)
        {
            iconOptions.SetActive(selected == buttonOptions);
        }

        if (iconExit)
        {
            iconExit.SetActive(selected == buttonExit);
        }

        if (GlobalInputManager.Instance.DeliverPressed()) // reemplazar con tu binding
        {
            if (selected == buttonPlay && sceneChanger != null)
            {
                sceneChanger.ChangeScene();
            }
            else if (selected == buttonOptions)
            {
                // abrir opciones
            }
            else if (selected == buttonExit)
            {
                Application.Quit();
            }
        }
    }
}
