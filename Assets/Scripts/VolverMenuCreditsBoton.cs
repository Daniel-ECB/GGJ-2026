using UnityEngine;

public class VolverMenuCreditsBoton : MonoBehaviour
{
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

        anim1.Play("Tomenu");
    }
}
