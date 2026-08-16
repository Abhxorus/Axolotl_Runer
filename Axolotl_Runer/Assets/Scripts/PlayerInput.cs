using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInput : MonoBehaviour
{
    [Header("Ajustes Táctiles (Móviles)")]
    public float swipeThreshold = 50f;

    private PlayerMovement player;
    private Vector2 startTouchPosition;
    private bool isSwiping = false;

    void Start()
    {
        // Conectamos con el script de movimiento automáticamente
        player = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (player.isDead) return;

        HandlePCInputs();
        HandleMobileInputs();
    }

    private void HandlePCInputs()
    {
        if (Keyboard.current == null) return;

        // Movimiento Lateral
        if (player.tipoMovimiento == ModoMovimiento.Libre)
        {
            float inputX = 0f;
            if (Keyboard.current.aKey.isPressed) inputX = -1f;
            else if (Keyboard.current.dKey.isPressed) inputX = 1f;
            player.SetInputX(inputX);
        }
        else // Modo Carriles
        {
            if (Keyboard.current.aKey.wasPressedThisFrame) player.ChangeLane(-1);
            else if (Keyboard.current.dKey.wasPressedThisFrame) player.ChangeLane(1);
        }

        // Acciones
        if (Keyboard.current.spaceKey.wasPressedThisFrame) player.TriggerJump();
        if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame) player.TriggerRoll();

        // Ataque
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            player.PerformAttack();
        }
    }

    private void HandleMobileInputs()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            startTouchPosition = touch.position.ReadValue();
            isSwiping = true;
        }

        if (player.tipoMovimiento == ModoMovimiento.Libre)
        {
            if (touch.press.isPressed)
            {
                float deltaX = touch.delta.ReadValue().x;
                player.SetInputX(Mathf.Clamp(deltaX / 10f, -1f, 1f));
            }
            else
            {
                player.SetInputX(0f); // Detiene el movimiento si sueltas
            }
        }

        if (touch.press.wasReleasedThisFrame && isSwiping)
        {
            Vector2 endTouchPosition = touch.position.ReadValue();
            Vector2 swipeDelta = endTouchPosition - startTouchPosition;

            if (swipeDelta.magnitude > swipeThreshold)
            {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    if (player.tipoMovimiento == ModoMovimiento.Carriles)
                    {
                        if (swipeDelta.x > 0) player.ChangeLane(1);
                        else player.ChangeLane(-1);
                    }
                }
                else
                {
                    if (swipeDelta.y > 0) player.TriggerJump();
                    else player.TriggerRoll();
                }
            }
            else
            {
                player.PerformAttack();
            }

            isSwiping = false;
        }
    }
}