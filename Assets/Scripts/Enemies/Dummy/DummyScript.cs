using UnityEngine;

public class DummyScript : MonoBehaviour
{

    private bool inRange;

    [Header("Interacción")]
    public GameObject interactIcon;

    void Start()
    {
        interactIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
            interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
            interactIcon.SetActive(false);
        }
    }
}
