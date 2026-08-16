using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Lista de tipos de PowerUps disponibles
    public enum PowerUpType { VidaExtra, SuperVelocidad }

    [Header("Configuración")]
    public PowerUpType type;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        if (health != null && movement != null)
        {
            // Aplicamos el efecto dependiendo de lo que se eligió en el Inspector
            switch (type)
            {
                case PowerUpType.VidaExtra:
                    health.Heal(1);
                    break;
                case PowerUpType.SuperVelocidad:
                    movement.ActivateSpeedBoost();
                    break;
            }

            // Destruimos el Power-Up tras ser recogido
            gameObject.SetActive(false);
        }
    }
}