using UnityEngine;

public class CreditosBoton : MonoBehaviour
{
    public GameObject objSelected;
    [SerializeField] private Animator anim1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        //aparecer nombres y anim

        anim1.Play("Tocredits");
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
