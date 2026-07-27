using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue = 10;

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el que chocó fue el jugador (usando el componente de vida como identificador)
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            // Buscamos el manager y sumamos los puntos
            ScoreManager scoreManager = Object.FindAnyObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.AddPoints(scoreValue);
            }

            // Destruimos la moneda
            Destroy(gameObject);
        }
    }
}