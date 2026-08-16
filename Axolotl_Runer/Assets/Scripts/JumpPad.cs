using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Configuración del Trampolín")]
    [Tooltip("Fuerza con la que saldrá volando el ajolote")]
    public float bouncePower = 12.0f;

    // Usamos OnCollisionEnter para que actúe en el instante en que los pies lo tocan
    private void OnCollisionEnter(Collision collision)
    {
        // Revisamos si el objeto que nos pisó tiene el script PlayerMovement
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

        if (player != null)
        {
            // Le damos la orden de rebotar usando la fuerza que configuramos aquí
            player.ForceBounce(bouncePower);
        }
    }
}