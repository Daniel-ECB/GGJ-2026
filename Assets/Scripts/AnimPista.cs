using UnityEngine;
using System.Collections;

public class AnimPista : MonoBehaviour
{
    [SerializeField] Vector3 startScale;
    [SerializeField] Vector3 targetScale;
    [SerializeField] float durationLerp = 0;
    void Start()
    {
        //startScale = transform.localScale;

        StartCoroutine(LerpScale(startScale, targetScale, durationLerp));
    }

    // Update is called once per frame
    void Update()
    {
    
    }

   IEnumerator LerpScale(Vector3 start, Vector3 target, float lerpDuration)
    {
        float timeElapsed = 0f;
        

        while (timeElapsed < durationLerp)
        {
            transform.localScale = Vector3.Lerp(start, target, timeElapsed / durationLerp);
            //Debug.Log(current);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
      
    }


}
