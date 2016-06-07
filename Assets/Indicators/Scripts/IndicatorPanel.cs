using UnityEngine;
using System.Collections;

public class IndicatorPanel : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-Assigned Variables")]
    public GameObject OffScreen;
    public GameObject OnScreen;
    public GameObject TargetCam;

    //  TRANSITION ANIMATIONS
    public void ScaleTransition(Transform target, Vector2 startSize, Vector3 endSize, float duration)
    {
        //StopAllCoroutines();
        StartCoroutine(CoScaleTransition(target, startSize, endSize, duration));
    }

    IEnumerator CoScaleTransition(Transform target, Vector3 startSize, Vector3 endSize, float duration)
    {
        //Debug.Log("Start Animate OffScreen");

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

        //Debug.Log("Done Animate OffScreen");
    }
}
