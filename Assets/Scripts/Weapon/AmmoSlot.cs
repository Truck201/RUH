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
    public Sprite spriteCount1;
    public Sprite spriteCount3;
    public Sprite spriteCount5;
    public Sprite spriteCount8;

    public void Clear()
    {
        name = "";
        icon = spriteEmpty;
        projectilePrefab = null;
        count = 0;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(name) || projectilePrefab == null || count <= 0;
    }

    // 🔹 Devuelve el sprite correcto según la cantidad
    public Sprite GetSpriteForCount()
    {
        if (count <= 0) return spriteEmpty;
        if (count >= 8) return spriteCount8;
        if (count >= 5) return spriteCount5;
        if (count >= 3) return spriteCount3;
        if (count >= 1) return spriteCount1;
        return spriteEmpty;
    }
}
