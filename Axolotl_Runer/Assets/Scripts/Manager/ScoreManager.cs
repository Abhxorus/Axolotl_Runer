using UnityEngine;
using TMPro; // Obligatorio para poder usar TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public int currentScore = 0;

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI scoreText; // La referencia a tu texto en el Canvas

    void Start()
    {
        // Nos aseguramos de que el texto muestre cero desde el principio
        ActualizarTexto();
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        ActualizarTexto();
    }

    // Separé esto en una función para mantenerlo ordenado
    private void ActualizarTexto()
    {
        // Solo intentamos cambiar el texto si la variable no está vacía
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore.ToString();
        }
        else
        {
            Debug.LogWarning("¡Falta asignar el texto del Score en el Inspector!");
        }
    }
}