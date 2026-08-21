using System.Collections; // Necesario para las corrutinas
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class PlayerHealth : MonoBehaviour
{
    [Header("Ajustes de Vida")]
    public int maxLives = 3;
    private int currentLives;

    [Header("Interfaz (UI)")]
    public Image[] fullHearts;

    [Header("Ajustes de Game Over")]
    [Tooltip("Tiempo de espera antes de volver al menú")]
    public float delayBeforeMenu = 2.5f;
    [Tooltip("Nombre exacto de tu escena de menú principal")]
    public string menuSceneName = "MenuScene";

    [Header("Sonidos")]
    public AudioClip sonidoDano;
    public AudioClip sonidoGameOver;

    private Animator anim;

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();

        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentLives -= damage;
        Debug.Log("¡Dano recibido! Vidas restantes: " + currentLives);

        CameraFollow camScript = Camera.main.GetComponent<CameraFollow>();
        if (camScript != null)
        {
            // 0.2 segundos de duración y 0.3 de fuerza (puedes ajustar estos valores)
            camScript.TriggerShake(0.2f, 0.3f);
        }

        if (currentLives < 0)
        {
            currentLives = 0;
        }

        if (currentLives > 0 && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sonidoDano);
        }

        UpdateHeartsUI();

        if (currentLives <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentLives += healAmount;

        if (currentLives > maxLives)
        {
            currentLives = maxLives;
        }

        UpdateHeartsUI();
        Debug.Log("¡Vida recuperada! Vidas actuales: " + currentLives);
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < fullHearts.Length; i++)
        {
            if (i < currentLives)
            {
                fullHearts[i].enabled = true;
            }
            else
            {
                fullHearts[i].enabled = false;
            }
        }
    }

    private void Die()
    {
        // En lugar de ejecutar todo de golpe, iniciamos la corrutina de Game Over
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        Debug.Log("¡El ajolote se ha quedado sin vidas! Fin del juego.");

        // 1. Detener todo el movimiento (como lo tenías antes)
        SegmentGenerator generator = FindAnyObjectByType<SegmentGenerator>();
        if (generator != null)
        {
            generator.isGameOver = true;
        }

        if (anim != null) anim.SetTrigger("Die");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sonidoGameOver);
        }

        LevelStreamer streamer = FindAnyObjectByType<LevelStreamer>();
        if (streamer != null)
        {
            streamer.isGameOver = true;
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // 2. Registrar el progreso
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (GameplayManager.Instance != null && DataManager.Instance != null)
        {
            DataManager.Instance.SaveScore(GameplayManager.Instance.currentScore);
        }

        // 3. Esperar el tiempo especificado (x tiempo)
        yield return new WaitForSeconds(delayBeforeMenu);

        if (DataManager.Instance != null)
        {
            DataManager.Instance.showRecordsOnLoad = true;
        }

        // 4. Volver al menú principal
        TransitionManager.Instance.CargarEscena(menuSceneName);
    }
}