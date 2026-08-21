using UnityEngine;

public class RotarIcono : MonoBehaviour
{
    public float velocidadGiro = 200f;

    void Update()
    {
        // Gira constantemente en el eje Z (negativo para girar a la derecha)
        transform.Rotate(0f, 0f, -velocidadGiro * Time.deltaTime);
    }
}