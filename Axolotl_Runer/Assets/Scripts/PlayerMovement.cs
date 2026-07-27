using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float lateralSpeed = 5.0f;
    public float lateralLimit = 5.0f;

    [Header("Ajustes de Salto")]
    public float jumpForce = 7.0f;
    public float gravity = -15.0f;
    public float groundLevel = 0f; // Ajusta esto a la altura de tu suelo

    private float verticalVelocity;
    private bool isGrounded;

    void Update()
    {
        HandleLateralMovement();
        HandleJump();
    }

    private void HandleLateralMovement()
    {
        if (Keyboard.current == null) return;

        float targetX = transform.position.x;

        // Leemos el input horizontal
        if (Keyboard.current.aKey.isPressed)
        {
            targetX -= lateralSpeed * Time.deltaTime;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            targetX += lateralSpeed * Time.deltaTime;
        }

        // Limitamos la posición para no salir del cauce
        targetX = Mathf.Clamp(targetX, -lateralLimit, lateralLimit);

        // Aplicamos la posición lateral
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }

    private void HandleJump()
    {
        if (Keyboard.current == null) return;

        // Comprobamos si el personaje está tocando el suelo
        if (transform.position.y <= groundLevel)
        {
            // Corregimos la posición para que no se hunda
            transform.position = new Vector3(transform.position.x, groundLevel, transform.position.z);
            verticalVelocity = 0f;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Aplicamos la gravedad constantemente si está en el aire
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Detectamos el input de salto (solo el instante en que se presiona)
        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
        }

        // Aplicamos el movimiento vertical al transform
        transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);
    }

    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }

    private System.Collections.IEnumerator SpeedBoostRoutine()
    {
        // Guardamos la velocidad original por si acaso
        float originalSpeed = lateralSpeed;

        // Aumentamos la velocidad
        lateralSpeed *= 2f;
        Debug.Log("¡Súper velocidad activada!");

        // Esperamos 5 segundos
        yield return new WaitForSeconds(5f);

        // Regresamos a la normalidad
        lateralSpeed = originalSpeed;
        Debug.Log("Velocidad normalizada.");
    }
}