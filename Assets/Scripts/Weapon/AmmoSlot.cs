using UnityEngine;

[System.Serializable]
public class AmmoSlot
{
    public string name;
    public Sprite icon; // 🔹 este va a cambiar dinámicamente según la cantidad
    public int count;
    public GameObject projectilePrefab;

    [Header("Sprites por cantidad")]
    public Sprite spriteEmpty;
    public Sprite sprite1;
    public Sprite sprite3;
    public Sprite sprite5;

    public void Clear()
    {
        name = "";
        icon = spriteEmpty;
        projectilePrefab = null;
        count = 0;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(name);
    }

    // 🔹 Devuelve el sprite correcto según la cantidad
    public Sprite GetSpriteForCount()
    {
        if (count <= 0) return spriteEmpty;
        if (count >= 5) return sprite5;
        if (count >= 3) return sprite3;
        if (count >= 1) return sprite1;
        return spriteEmpty;
    }
}
