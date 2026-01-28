using UnityEngine;

public class MovPersonaje : MonoBehaviour
{
    public float VelAdelante = 5f;
    public float VelLado = 8f;
    

    void Update()
    {
        float Cambiar = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.forward * VelAdelante * Time.deltaTime);

        transform.Translate(Vector3.right * Cambiar * VelLado * Time.deltaTime);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Derecha"))
        {
            transform.Rotate(0f, 90f, 0f);
        }
    }

}
