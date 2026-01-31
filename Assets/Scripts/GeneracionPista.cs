using UnityEngine;
using System.Collections.Generic;

public class GeneracionPista : MonoBehaviour

{
    public GameObject Player;
    public GameObject[] calles;

    private List<GameObject> callesInstanciadas = new List<GameObject>();

    public float posZ;
    public float posY;

    [SerializeField] private int[] ordenGeneracion;
    private int indiceSecuencia = 0;

    void Update()
    {
        GenerarPista();
        EliminarCallesViejas();
    }

    public void GenerarPista()
    {
        if (Player.transform.position.z > posZ - 80)
        {
            int indicePrefab = ordenGeneracion[indiceSecuencia];

            GameObject nueva = Instantiate(
                calles[indicePrefab],
                new Vector3(0, posY, posZ + 80),
                Quaternion.identity
            );

            callesInstanciadas.Add(nueva);

            posZ += 50;

            indiceSecuencia++;

            // si llega al final, frena o reinicia (tu elección)
            if (indiceSecuencia >= ordenGeneracion.Length)
                indiceSecuencia = 0; // o eliminar esta línea si NO querés loop
        }
    }

    void EliminarCallesViejas()
    {
        float distanciaDestruir = 120f;

        for (int i = callesInstanciadas.Count - 1; i >= 0; i--)
        {
            if (Player.transform.position.z - callesInstanciadas[i].transform.position.z > distanciaDestruir)
            {
                Destroy(callesInstanciadas[i]);
                callesInstanciadas.RemoveAt(i);
            }
        }
    }


}

