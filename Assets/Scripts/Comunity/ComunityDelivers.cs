using UnityEngine;

[CreateAssetMenu(fileName = "NewComunityDeliver", menuName = "ScriptableObjects/ComunityDelivers")]
public class ComunityDeliver : ScriptableObject
{
    [System.Serializable]
    public class DeliverLevel
    {
        [Header("Verduras Pedidas")]
        public int zanahorias;
        public int papas;
        public int cebollas;

        [Header("Experiencia")]
        public int experiencia;
    }

    [System.Serializable]
    public class LevelGroup
    {
        [Header("Pedidos disponibles para este nivel")]
        public DeliverLevel[] pedidos = new DeliverLevel[6];
    }

    [Header("Pedidos por Nivel (0 a 5)")]
    public LevelGroup[] niveles = new LevelGroup[6];
}
