using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 7.0f;
    public float horizontalSpeed = 3;
    public float rightLimit = 5;
    public float leftLimit = -5;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
            {
                if (this.gameObject.transform.position.x > leftLimit)
                {
                    transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed);
                }
            }
            if (Keyboard.current.dKey.isPressed)
            {
                if (this.gameObject.transform.position.x  < rightLimit)
                {
                    transform.Translate(Vector3.left * Time.deltaTime * horizontalSpeed * -1);
                }

            }
        }
    }
}
