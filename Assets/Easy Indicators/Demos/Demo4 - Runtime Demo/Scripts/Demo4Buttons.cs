using UnityEngine;
using System.Collections;

public class Demo4Buttons : MonoBehaviour
{
    IndicatorViewer viewer;

    void Awake()
    {
        viewer = FindObjectOfType<IndicatorViewer>();
    }

    public void StartTracking()
    {
        viewer.SetTracking(true, true);
    }
    public void StopTracking()
    {
        viewer.SetTracking(false, false);
    }
}
