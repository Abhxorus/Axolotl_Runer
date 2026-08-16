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

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();
    }

    public void TakeDamage(int damage)
    {
        currentLives -= damage;
        Debug.Log("¡Daño recibido! Vidas restantes: " + currentLives);

        if (currentLives < 0)
        {
            currentLives = 0;
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
        if (scoreManager != null && DataManager.Instance != null)
        {
            // Sacamos el puntaje del ScoreManager y lo guardamos en el Singleton
            DataManager.Instance.SaveScore(scoreManager.currentScore);
            Debug.Log("Puntaje guardado: " + scoreManager.currentScore);
        }

        // 3. Esperar el tiempo especificado (x tiempo)
        yield return new WaitForSeconds(delayBeforeMenu);

        // 4. Volver al menú principal
        SceneManager.LoadScene(menuSceneName);
    }
}