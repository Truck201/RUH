using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Ajuste de profundidad")]
    public int sortingOrderBase = 5000;   // valor base alto
    public float offset = 0f;             // offset opcional si tu pivote no está centrado

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // 🔹 Calcula el orden en base a la posición Y
        spriteRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
    }
}
