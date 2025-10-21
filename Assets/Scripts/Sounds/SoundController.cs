// Adaptado SoundController para manejar Fade, escenas y mixer
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    [Header("Fuentes de Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    private AudioSource loopSfxSource;

    [Header("Audio Mixer")]
    public AudioMixer mixerMaster;

    [Header("Clips de Música Level 1")]
    public AudioClip music_Menu;
    public AudioClip music_Game;
    public AudioClip music_Caves;

    public AudioClip music_Battle;

    [Header("SFX Game")]
    [Header("HUD")]
    public AudioClip SFX_move;
    public AudioClip SFX_select;
    public AudioClip SFX_pause;

    [Header("Player")]
    public AudioClip SFX_absorb;
    public AudioClip SFX_noAbsorb;

    public AudioClip SFX_shoot;
    public AudioClip SFX_getDamage;
    public AudioClip SFX_pickup;
    public AudioClip SFX_winExperience;

    public AudioClip SFX_walk_grass;
    public AudioClip SFX_walk_grave;

    public AudioClip SFX_newLevel;

    [Header("Enemy")]
    public AudioClip SFX_hitEnemy;
    public AudioClip SFX_deathEnemy;
    public AudioClip SFX_detected;

    [Header("NPC")]
    public AudioClip SFX_dialog_0;
    public AudioClip SFX_dialog_1;

    [Header("Delivers")]
    public AudioClip SFX_newsDelivers;
    public AudioClip SFX_delivered;
    public AudioClip SFX_cannotDeliver;

    [Header("Train")]
    public AudioClip SFX_repairTrain;
    public AudioClip SFX_claxonTrain;

    [Header("Ambient")]
    [Header("Forest")]
    public AudioClip SFX_birds_0;
    public AudioClip SFX_birds_1;

    public AudioClip SFX_wind_ambient_forest_0;
    public AudioClip SFX_wind_ambient_forest_1;

    [Header("Caves")]
    public AudioClip SFX_leak_0;
    public AudioClip SFX_leak_1;

    public AudioClip SFX_wind_ambient_caves_0;
    public AudioClip SFX_wind_ambient_caves_1;

    [Header("Configuración Fade")]
    public float fadeDuration = 1.5f;

    private Coroutine currentFadeCoroutine;
    private float masterVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();

            // ✅ Crear la fuente de Audio para loops
            loopSfxSource = gameObject.AddComponent<AudioSource>();
            loopSfxSource.loop = true;
            loopSfxSource.volume = sfxSource.volume;
            loopSfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔹 Método para reproducir un sonido en loop
    public void PlaySFXLoop(AudioClip clip)
    {
        if (clip == null) return;

        if (loopSfxSource.isPlaying && loopSfxSource.clip == clip)
            return; // si ya se está reproduciendo ese loop, no lo repitas

        loopSfxSource.clip = clip;
        loopSfxSource.volume = sfxSource.volume;
        loopSfxSource.Play();
    }

    // 🔹 Método para detener el sonido en loop
    public void StopSFXLoop()
    {
        if (loopSfxSource.isPlaying)
            loopSfxSource.Stop();
    }


    public void PlayMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip && musicSource.isPlaying)
            return;

        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeOutIn(newClip));
    }

    public void StopMusic()
    {
        if (currentFadeCoroutine != null)
            StopCoroutine(currentFadeCoroutine);

        currentFadeCoroutine = StartCoroutine(FadeOut());
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip)
            sfxSource.PlayOneShot(clip);
    }

    private IEnumerator FadeOutIn(AudioClip newClip)
    {
        yield return FadeOut();

        musicSource.clip = newClip;

        if (musicSource.enabled && musicSource.gameObject.activeInHierarchy)
            musicSource.Play();
        else
            Debug.LogWarning("musicSource está desactivado y no puede reproducirse.");

        yield return FadeIn();
    }


    private IEnumerator FadeOut()
    {
        float startVolume = musicSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = 0f;
        musicSource.Stop();
    }

    private IEnumerator FadeIn()
    {
        float targetVolume = 1f;
        musicSource.volume = 0f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        float volumeDb = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20f;
        mixerMaster?.SetFloat("MasterVolume", volumeDb);
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(masterVolume);
    }

    public AudioClip GetSceneMusic(string sceneName)
    {
        return sceneName switch
        {
            "MainMenu" => music_Menu,
            "Farm" => music_Game,
            "ExtendsCaves" => music_Caves,
            _ => null
        };
    }

    public void PlaySceneMusic()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        AudioClip sceneMusic = GetSceneMusic(currentScene);

        if (sceneMusic != null)
        {
            musicSource.loop = true; // La música debe ir en loop
            PlayMusic(sceneMusic);
        }
    }

    // 🔹 Reproducir un sonido de ambiente en loop
    public void PlayAmbientLoop(AudioClip clip)
    {
        if (clip == null) return;

        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    // 🔹 Detener sonido de ambiente en loop
    public void StopAmbientLoop()
    {
        if (ambientSource.isPlaying)
            ambientSource.Stop();
    }
}
