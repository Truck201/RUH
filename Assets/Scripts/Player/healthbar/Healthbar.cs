using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image energyBar;

    [SerializeField] private SpriteRenderer health_1;
    [SerializeField] private SpriteRenderer health_2;
    [SerializeField] private SpriteRenderer health_3;
    [SerializeField] private SpriteRenderer health_4;
    [SerializeField] private SpriteRenderer health_5;

    private int vidaActual;
    private float estaminaActual;
    private int experienciaActual;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] PlayerMovement player;

    private void Awake()
    {
        if (!playerStats)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (!player)
            player = FindFirstObjectByType<PlayerMovement>();
    }

    void Start()
    {
        vidaActual = playerStats.vidas;
        estaminaActual = playerStats.estamina;
        experienciaActual = playerStats.experiencia;

        Debug.Log($"|| Vidas = {vidaActual} || Estamina = {estaminaActual} || Experiencia {experienciaActual} ||");
    }

    void Update()
    {
        UpdateCurrentLifeUI();
        UpdateCurrentExperienceUI();
        UpdateCurrentEstamina();
    }

    private void UpdateCurrentLifeUI()
    {
        float vidaNormalizada = Mathf.Clamp01((float)vidaActual / 5f);
        //healthBar.fillAmount = vidaNormalizada;
    }

    private void UpdateCurrentExperienceUI()
    {
        if (playerStats.experiencia >= playerStats.experienciaLevel)
        {
            if (playerStats.nivelActual <= 4)
            {
                playerStats.levelUP = true;
            }
        }

        if (playerStats.experienciaImage)
            playerStats.experienciaImage.fillAmount = playerStats.experiencia / playerStats.experienciaLevel;
    }

    private void UpdateCurrentEstamina()
    {
        if (!player) return;

        // 🔹 pedimos el valor normalizado (1 → 0)
        float staminaNormalized = player.GetStaminaNormalized();

        if (energyBar)
            energyBar.fillAmount = staminaNormalized;
    }
}
