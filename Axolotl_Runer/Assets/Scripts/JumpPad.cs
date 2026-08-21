using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Configuración del Trampolín")]
    public float bouncePower = 12f;

    // 1. Se ejecuta si la plataforma tiene "Is Trigger" marcado (Como tu Trajinera)
    private void OnTriggerEnter(Collider other)
    {
        EvaluarSalto(other.gameObject);
    }

    // 2. Se ejecuta si la plataforma es sólida (Como un trampolín en el suelo)
    private void OnCollisionEnter(Collision collision)
    {
        EvaluarSalto(collision.gameObject);
    }

    // Lógica central para decidir si impulsar o no
    private void EvaluarSalto(GameObject objetoImpactado)
    {
        PlayerMovement player = objetoImpactado.GetComponent<PlayerMovement>();
        Rigidbody rb = objetoImpactado.GetComponent<Rigidbody>();

        if (player != null && rb != null)
        {
            // Comprobamos la velocidad en el eje Y
            // Si es menor o igual a 0.1, significa que está cayendo o caminando en plano.
            // Si es mayor a 0.1, está saltando hacia arriba, así que lo ignoramos.
            if (rb.linearVelocity.y <= 0.1f)
            {
                player.ForceBounce(bouncePower);
            }
        }
    }
}