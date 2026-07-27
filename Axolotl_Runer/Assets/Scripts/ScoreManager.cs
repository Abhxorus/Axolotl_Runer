using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int currentScore = 0;

    public void AddPoints(int points)
    {
        currentScore += points;
        Debug.Log("Puntuación actual: " + currentScore);
        // Aquí conectaremos un texto del Canvas más adelante
    }
}