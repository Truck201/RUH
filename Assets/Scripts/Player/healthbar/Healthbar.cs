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
    private int estaminaActual;
    private int experienciaActual;

    [SerializeField] private PlayerStats playerStats;
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
    }

    private void UpdateCurrentLifeUI()
    {
        float vidaNormalizada = Mathf.Clamp01((float)vidaActual / 5f);
        healthBar.fillAmount = vidaNormalizada;
    }

    private void UpdateCurrentExperienceUI()
    {

    }
}
