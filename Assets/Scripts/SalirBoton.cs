using UnityEngine;

public class SalirBoton : MonoBehaviour
{
    public GameObject objSelected;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Application.Quit();
    }

    private void OnMouseOver()
    {
        objSelected.SetActive(true);
    }

    private void OnMouseExit()
    {
        objSelected.SetActive(false);
    }
}
