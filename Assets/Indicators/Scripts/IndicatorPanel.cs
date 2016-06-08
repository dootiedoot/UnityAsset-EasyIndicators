using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorPanel : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-Assigned Variables")]
    public GameObject OffScreen;
    public GameObject OnScreen;
    //public GameObject TargetCam;

    //  TRANSITION ANIMATIONS
    public void ScaleTransition(Transform target, Vector2 startSize, Vector3 endSize, float duration)
    {
        StartCoroutine(CoScaleTransition(target, startSize, endSize, duration));
    }
    public void FadeTransition(Transform target, int targetAlpha, float duration)
    {
        StartCoroutine(CoFadeTransition(target, targetAlpha, duration));
    }

    //  Coroutine for animating the scale of a target's indicator from a starting size to an ending size with a duration.
    IEnumerator CoScaleTransition(Transform target, Vector3 startSize, Vector3 endSize, float duration)
    {
        float ratio = 0;
        float multiplier = 1 / duration;
       
        target.localScale = startSize;

        while (target.localScale != endSize)
        {
            //  Increment time
            ratio += Time.deltaTime * multiplier;

            //  Adjust scale using Lerp
            target.localScale = Vector3.Lerp(startSize, endSize, ratio);

            yield return null;
        }

        if (endSize == Vector3.zero)
            target.gameObject.SetActive(false);
    }

    //  Coroutine for animating the alpha of a target's indicator from a starting size to an ending size with a duration.
    IEnumerator CoFadeTransition(Transform target, int targetAlpha, float duration)
    {
        //  Find each graphic object and store it. Includes all images, texts, etc.
        Graphic[] graphics = target.GetComponentsInChildren<Graphic>(true);

        if (graphics.Length > 0)
            for (int i = 0; i < graphics.Length; i++)
            {
                //  Initial set-up for the alpha to work with CrossFadeAlpha
                if (targetAlpha >= 1)
                    graphics[i].canvasRenderer.SetAlpha(0);
                else
                    graphics[i].canvasRenderer.SetAlpha(1);
                  
                //  Use the CrossFadeAlpha to do fading transition
                graphics[i].CrossFadeAlpha(targetAlpha, duration, false);
            }

        yield return new WaitForSeconds(duration);
        
        //  if the target alpha is 0 (transparent), disable the indicator
        if (targetAlpha <= 0)
            target.gameObject.SetActive(false);
    }
}
