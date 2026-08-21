using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            // Usamos el nuevo Singleton de la partida
            if (GameplayManager.Instance != null)
            {
                GameplayManager.Instance.AddPoints(scoreValue);
            }

            gameObject.SetActive(false);
        }
    }
}