using UnityEngine;

public class DamageObstacle : MonoBehaviour
{
    public int damageAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Buscamos si el objeto con el que chocamos tiene el sistema de vida
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damageAmount);

            // Opcional: Destruir el obstáculo después del impacto para no recibir daño doble
            Destroy(gameObject);
        }
    }
}