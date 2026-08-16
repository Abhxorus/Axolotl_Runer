using System.Collections;
using UnityEngine;

public enum ModoMovimiento { Libre, Carriles }

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public ModoMovimiento tipoMovimiento = ModoMovimiento.Carriles;
    public float lateralSpeed = 5.0f;
    public float lateralLimit = 5.0f;
    public float laneChangeSpeed = 15.0f;
    private int currentLane = 0;

    [Header("Ajustes de Salto y Rodado")]
    public float jumpForce = 7.0f;
    public float fastFallForce = 15.0f;
    public float rollDuration = 0.8f;
    public float rolledHeight = 0.5f;

    [Header("Ajustes de Ataque")]
    public float bounceForce = 8.0f;
    public float attackRange = 1.0f;
    public float attackRadius = 1.5f;
    public LayerMask hittableLayer;

    [Header("Ajustes de Suelo y Agua")]
    public LayerMask groundLayer;
    public float waterLevel = -2.0f;

    [HideInInspector] public bool isDead = false;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private bool isGrounded = false;

    private float originalHeight;
    private Vector3 originalCenter;
    private bool isRolling = false;
    private float rollTimer = 0f;

    private float inputX = 0f;
    private bool jumpPressed = false;
    private bool rollPressed = false;
    private bool bouncePending = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        if (playerCollider != null)
        {
            originalHeight = playerCollider.height;
            originalCenter = playerCollider.center;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isRolling)
        {
            rollTimer -= Time.deltaTime;
            if (rollTimer <= 0) StopRoll();
        }

        CheckWater();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        HandleLateralMovement();
        HandleJump();
        HandleRollAndFastFall();
        HandleBounce();
    }

    // --- MÉTODOS PÚBLICOS PARA EL INPUT ---

    public void SetInputX(float x)
    {
        inputX = x;
    }

    public void ChangeLane(int direction)
    {
        currentLane += direction;
        currentLane = Mathf.Clamp(currentLane, -1, 1);
    }

    public void TriggerJump()
    {
        jumpPressed = true;
    }

    public void TriggerRoll()
    {
        rollPressed = true;
    }

    public void PerformAttack()
    {
        Vector3 attackPos = transform.position + (Vector3.forward * attackRange);
        Collider[] hitObjects = Physics.OverlapSphere(attackPos, attackRadius, hittableLayer);

        bool hitSuccess = false;
        foreach (Collider obj in hitObjects)
        {
            hitSuccess = true;
            obj.gameObject.SetActive(false);
        }

        if (hitSuccess && !isGrounded) bouncePending = true;
    }

    public void ForceBounce(float customForce)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * customForce, ForceMode.Impulse);
        isGrounded = false;
    }

    // --- FÍSICAS INTERNAS ---

    private void HandleLateralMovement()
    {
        if (tipoMovimiento == ModoMovimiento.Libre)
        {
            rb.linearVelocity = new Vector3(inputX * lateralSpeed, rb.linearVelocity.y, 0f);
            Vector3 clampedPos = rb.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, -lateralLimit, lateralLimit);
            rb.position = clampedPos;
        }
        else
        {
            float targetX = currentLane * lateralLimit;
            float newX = Mathf.MoveTowards(rb.position.x, targetX, laneChangeSpeed * Time.fixedDeltaTime);
            rb.position = new Vector3(newX, rb.position.y, rb.position.z);
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }

    private void HandleJump()
    {
        if (jumpPressed)
        {
            jumpPressed = false;
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
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.down * fastFallForce, ForceMode.Impulse);
            }
            else if (isGrounded && !isRolling)
            {
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

    private void HandleBounce()
    {
        if (bouncePending)
        {
            bouncePending = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0) isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0) isGrounded = false;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 attackPos = transform.position + (Vector3.forward * attackRange);
        Gizmos.DrawWireSphere(attackPos, attackRadius);
    }
}