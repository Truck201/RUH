using UnityEngine;

public class CropPlot : MonoBehaviour
{
    [Header("Sprites de cultivo según tipo de piedra (0, 1, 2)")]
    public Sprite[] stoneSprites; // índice 0,1,2 = tipos de piedra

    [Header("Slots de cultivo (en grilla)")]
    public SpriteRenderer[] cropSlots; // cada uno es una celda del cultivo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StoneProjectile projectile = collision.GetComponent<StoneProjectile>();
        if (projectile != null && projectile.isProyectible)
        {
            CollectibleStone stoneData = projectile.GetComponent<CollectibleStone>();
            if (stoneData != null && stoneData.slotIndex >= 0 && stoneData.slotIndex < stoneSprites.Length)
            {
                // Cambiar un slot aleatorio del cultivo
                int slotToChange = Random.Range(0, cropSlots.Length);
                cropSlots[slotToChange].sprite = stoneSprites[stoneData.slotIndex];
            }

            Destroy(collision.gameObject);
        }
    }
}
