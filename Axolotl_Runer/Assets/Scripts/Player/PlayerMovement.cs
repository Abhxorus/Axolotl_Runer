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

    [Header("Sonidos")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoCaida;
    public AudioClip sonidoRodar;
    public AudioClip sonidoGolpe;
    public AudioClip sonidoPogo;
    public AudioClip sonidoPasos;
    [Tooltip("Qué tan rápido suenan los pasos (en segundos)")]
    public float tiempoEntrePasos = 0.3f;
    private float pasoTimer = 0f;
    private AudioSource fuentePasos;

    [HideInInspector] public bool isDead = false;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private Animator anim;
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

        anim = GetComponentInChildren<Animator>();

        if (playerCollider != null)
        {
            originalHeight = playerCollider.height;
            originalCenter = playerCollider.center;
        }

        fuentePasos = gameObject.AddComponent<AudioSource>();
        fuentePasos.playOnAwake = false;
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

        if (isGrounded && !isRolling && !jumpPressed && !bouncePending)
        {
            pasoTimer -= Time.deltaTime;

            if (pasoTimer <= 0f)
            {
                if (fuentePasos != null && sonidoPasos != null)
                {
                    // Usamos la fuente local en lugar del AudioManager
                    fuentePasos.PlayOneShot(sonidoPasos);
                }
                pasoTimer = tiempoEntrePasos;
            }
        }
        else
        {
            // Resetea el timer para que suene justo al tocar el suelo
            pasoTimer = 0f;

            // ¡ESTO HACE QUE PARE DE GOLPE!
            if (fuentePasos != null && fuentePasos.isPlaying)
            {
                fuentePasos.Stop();
            }
        }
        if (anim != null)
        {
            anim.SetBool("isGrounded", isGrounded);
            anim.SetBool("isRolling", isRolling);
            // Pasamos la velocidad vertical para saber si sube o cae
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
        }
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
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(sonidoGolpe);

        if (anim != null) anim.SetTrigger("Attack");

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

        if (anim != null) anim.SetTrigger("Jump");
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
                if (anim != null) anim.SetTrigger("Jump");
                AudioManager.Instance.PlaySFX(sonidoSalto);
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

                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(sonidoCaida);
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
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(sonidoRodar);
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
            //if (anim != null) anim.SetTrigger("Jump");
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(sonidoPogo);
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