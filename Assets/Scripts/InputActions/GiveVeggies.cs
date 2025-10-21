using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class GiveVeggies : MonoBehaviour
{
    [Header("Configuración de la verdura")]
    public string veggieName;
    public Sprite veggieIcon;
    public int veggieAmount = 1;

    [Header("Sprite del objeto en el suelo")]
    public SpriteRenderer veggieSpriteRenderer;  // sprite de la planta en el suelo
    public Sprite emptySprite;                   // sprite vacío (después de recolectar)

    [Header("Interacción")]
    public GameObject interactIcon;

    [Header("Efecto de succión")]
    public Transform playerTransform;            // referencia al jugador
    public float suctionSpeed = 5f;              // velocidad de succión visual
    public GameObject veggieVisualPrefab;        // opcional: prefab visual que vuela al jugador

    private bool playerNear = false;
    private bool collected = false;

    private void Start()
    {
        if (interactIcon)
            interactIcon.SetActive(false);
    }

    private void Update()
    {
        if (collected || playerTransform == null)
            return;

        if (GlobalInputManager.Instance == null)
            return;

        // Mostrar interacción (opcional)
        if (playerNear && GlobalInputManager.Instance.DeliverPressed())
        {
            StartCoroutine(CollectVeggie());
        }
    }

    private IEnumerator CollectVeggie()
    {
        collected = true;

        interactIcon.SetActive(false);

        // Cambiar el sprite a vacío
        if (veggieSpriteRenderer != null && emptySprite != null)
        {
            veggieSpriteRenderer.sprite = emptySprite;
        } else if (emptySprite == null)
        {
            veggieSpriteRenderer.sprite = null;
        }

        // Crear el efecto de succión visual
        if (veggieVisualPrefab != null && playerTransform != null)
        {
            GameObject visual = Instantiate(veggieVisualPrefab, transform.position, Quaternion.identity);
            Vector3 startPos = visual.transform.position;
            Vector3 endPos = playerTransform.position;

            float t = 0f;
            while (t < 1f)
            {
                if (visual == null) yield break;
                t += Time.deltaTime * suctionSpeed;
                visual.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            Destroy(visual);
        }

        // Sumar al inventario
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddVeggie(veggieName, veggieIcon, veggieAmount);
        }

        Debug.Log($"Recolectaste {veggieAmount}x {veggieName} 🥬");

        yield return new WaitForSeconds(0.2f);
        // Si querés que desaparezca el objeto físico:
        // Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = true;
            interactIcon.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
            interactIcon.SetActive(false);
        }
    }
}
