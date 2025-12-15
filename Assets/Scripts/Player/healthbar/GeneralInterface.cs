using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralInterface : MonoBehaviour
{
    public static GeneralInterface Instance;
    [Header("General bar")]
    [SerializeField] private Image energyBar;

    [Header("Sprites lifes")]
    [SerializeField] private Image health_1;
    [SerializeField] private Image health_2;
    [SerializeField] private Image health_3;
    [SerializeField] private Image health_4;
    [SerializeField] private Image health_5;

    private Image[] healthImages;

    private int vidaActual;
    private float estaminaActual;
    private int experienciaActual;

    [Header("Text Experience")]
    [SerializeField] TMP_Text experienceText;

    [Header("Text Level")]
    [SerializeField] TMP_Text levelText;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] PlayerMovement player;

    private void Awake()
    {
        if (GeneralInterface.Instance == null)
        {
            GeneralInterface.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!playerStats)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (!player)
            player = FindFirstObjectByType<PlayerMovement>();

        // Guardamos todas las imágenes en un array para manejarlas fácilmente
        healthImages = new Image[] { health_1, health_2, health_3, health_4, health_5 };
    }

    void Start()
    {
        vidaActual = playerStats.vidas;
        estaminaActual = playerStats.estamina;
        experienciaActual = playerStats.experiencia;

        UpdateCurrentLifeUI();
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
        int vidas = Mathf.Clamp(playerStats.vidas, 0, 5);
        for (int i = 0; i < healthImages.Length; i++)
        {
            // Muestra solo las imágenes de vida activa
            healthImages[i].enabled = (i < vidas);
        }
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

        if (experienceText)
        {
            int percent = playerStats.experiencia / playerStats.experienciaLevel * 100;
            experienceText.SetText($"{percent}%");
        }
            
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
