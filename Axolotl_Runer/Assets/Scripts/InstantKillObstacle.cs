using UnityEngine;

public class InstantKillObstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            // Le quitamos todas las vidas posibles de un solo golpe
            player.TakeDamage(player.maxLives);
        }
    }
}