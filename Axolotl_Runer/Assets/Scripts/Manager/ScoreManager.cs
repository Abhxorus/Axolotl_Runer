using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Variables de Partida")]
    public int currentScore = 0;
    public TextMeshProUGUI scoreText; // Texto en el HUD del juego

    [Header("Variables de Menú de Récords")]
    public TextMeshProUGUI lastScoreText; // Texto pintado en la pared 3D
    public TextMeshProUGUI highScoreText; // Texto pintado en la pared 3D

    void Start()
    {
        // 1. Si estamos en partida, inicializa el texto en 0
        if (scoreText != null)
        {
            ActualizarTexto();
        }

        // 2. Si estamos en el menú, consulta el DataManager de forma exclusiva
        if (DataManager.Instance != null)
        {
            if (lastScoreText != null)
            {
                lastScoreText.text = "Puntaje: " + DataManager.Instance.lastScore.ToString();
            }

            if (highScoreText != null)
            {
                highScoreText.text = "Mejor Récord: " + DataManager.Instance.highScore.ToString();
            }
        }
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        ActualizarTexto();
    }

    private void ActualizarTexto()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore.ToString();
        }
    }
}