using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float lateralSpeed = 5.0f;
    public float lateralLimit = 5.0f;

    [Header("Ajustes de Salto y Rodado")]
    public float jumpForce = 7.0f;
    public float fastFallForce = 15.0f;
    public float rollDuration = 0.8f;
    public float rolledHeight = 0.5f; // Qué tan bajito se hace el ajolote al rodar

    [Header("Ajustes de Suelo y Agua")]
    public LayerMask groundLayer;
    public float waterLevel = -2.0f;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private bool isDead = false;
    private bool isGrounded = false;

    // Variables de estado original del collider
    private float originalHeight;
    private Vector3 originalCenter;
    private bool isRolling = false;
    private float rollTimer = 0f;

    // Variables de lectura de inputs
    private float inputX = 0f;
    private bool jumpPressed = false;
    private bool rollPressed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        // Guardamos las proporciones originales del personaje
        if (playerCollider != null)
        {
            originalHeight = playerCollider.height;
            originalCenter = playerCollider.center;
        }
        else
        {
            Debug.LogWarning("¡Falta un CapsuleCollider en el jugador para la mecánica de rodar!");
        }
    }

    void Update()
    {
        if (isDead) return;

        if (Keyboard.current != null)
        {
            inputX = 0f;
            if (Keyboard.current.aKey.isPressed) inputX = -1f;
            else if (Keyboard.current.dKey.isPressed) inputX = 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpPressed = true;
            }

            if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                rollPressed = true;
            }
        }

        // Temporizador para dejar de rodar
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0)
            {
                StopRoll();
            }
        }

        CheckWater();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        HandleLateralMovement();
        HandleJump();
        HandleRollAndFastFall();
    }

    private void HandleLateralMovement()
    {
        rb.linearVelocity = new Vector3(inputX * lateralSpeed, rb.linearVelocity.y, 0f);

        Vector3 clampedPos = rb.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -lateralLimit, lateralLimit);
        rb.position = clampedPos;
    }

    private void HandleJump()
    {
        if (jumpPressed)
        {
            jumpPressed = false;

            // Si está rodando, no puede saltar (opcional, puedes quitar la condición de !isRolling)
            if (isGrounded && !isRolling)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
    }

    private void HandleRollAndFastFall()
    {
        if (rollPressed)
        {
            rollPressed = false;

            if (!isGrounded)
            {
                // 1. Caída rápida si está en el aire
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Opcional: frena el salto seco
                rb.AddForce(Vector3.down * fastFallForce, ForceMode.Impulse);
            }
            else if (isGrounded && !isRolling)
            {
                // 2. Rodar si está en el suelo
                StartRoll();
            }
        }
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;

        if (playerCollider != null)
        {
            playerCollider.height = rolledHeight;
            // Ajustamos el centro hacia abajo para que los pies sigan tocando el suelo
            playerCollider.center = new Vector3(originalCenter.x, rolledHeight / 2f, originalCenter.z);
        }
    }

    private void StopRoll()
    {
        isRolling = false;

        if (playerCollider != null)
        {
            playerCollider.height = originalHeight;
            playerCollider.center = originalCenter;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }

    private void CheckWater()
    {
        if (transform.position.y < waterLevel)
        {
            isDead = true;
            PlayerHealth health = GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(health.maxLives);
        }
    }

    public void ActivateSpeedBoost()
    {
        StartCoroutine(SpeedBoostRoutine());
    }

    private IEnumerator SpeedBoostRoutine()
    {
        float originalSpeed = lateralSpeed;
        lateralSpeed *= 2f;

        yield return new WaitForSeconds(5f);

        lateralSpeed = originalSpeed;
    }
}