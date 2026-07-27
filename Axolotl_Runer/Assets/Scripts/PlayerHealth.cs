using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ajustes de Vida")]
    public int maxLives = 3;
    private int currentLives;

    [Header("Interfaz (UI)")]
    public Image[] fullHearts;

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

        // Evitamos tener más vidas que el máximo permitido
        if (currentLives > maxLives)
        {
            currentLives = maxLives;
        }

        UpdateHeartsUI();
        Debug.Log("¡Vida recuperada! Vidas actuales: " + currentLives);
    }

    private void UpdateHeartsUI()
    {
        // Recorremos la lista de corazones llenos
        for (int i = 0; i < fullHearts.Length; i++)
        {
            // Si el índice es menor a las vidas, encendemos la imagen.
            // Si es mayor o igual, la apagamos revelando el fondo oscuro.
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
        // Aquí conectaremos la pantalla de Game Over más adelante
        Debug.Log("¡El ajolote se ha quedado sin vidas! Fin del juego.");

        // Usamos la nueva sintaxis optimizada de Unity
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

        // Opcional: Detener también el script de PlayerMovement
        GetComponent<PlayerMovement>().enabled = false;
    }
}