using UnityEngine;
using UnityEngine.AI;

public class blockAccess : MonoBehaviour
{
    private NavMeshObstacle obstacle;

    private void Awake()
    {
        obstacle = gameObject.AddComponent<NavMeshObstacle>();

        // Configuración del obstáculo
        obstacle.carving = true;
        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.size = new Vector3(1.8f, 1f, 1f); // Ajustá el tamaño a tu bloque
        obstacle.carveOnlyStationary = false;
    }

    private void Start()
    {
        if (PlayerStats.Instance.nivelActual >= 2) DisableBlock();
    }

    // Llamar cuando querés activar el bloqueo
    public void ActivateBlock()
    {
        obstacle.enabled = true;
        gameObject.SetActive(true);
    }

    // Llamar cuando querés desactivar o hacer desaparecer el bloqueo
    public void DisableBlock()
    {
        obstacle.enabled = false;
        gameObject.SetActive(false);
    }

    public void RemoveBlock()
    {
        Destroy(obstacle);
    }
}
