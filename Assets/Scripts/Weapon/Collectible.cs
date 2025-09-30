using UnityEngine;

public enum CollectibleType
{
    Stone,
    Veggie
}

public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public string itemName = "Item";
    public Sprite itemIcon;
    public GameObject projectilePrefab; // solo se usa si es Stone
}
