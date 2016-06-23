using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Demo4Settings : MonoBehaviour
{
    public static GameObject currentTarget;
    IndicatorViewer viewer;
    Slider slider;
    Toggle toggle;

    void Awake()
    {
        viewer = FindObjectOfType<IndicatorViewer>();
        slider = GetComponent<Slider>();
        toggle = GetComponent<Toggle>();
    }

    //  VIEWER
    public void ToggleTracking()
    {
        if (toggle.isOn)
            IndicatorViewer.StartTracking();
        else
            IndicatorViewer.StopTracking();
    }

    public void ToggleOnScreen()
    {
        IndicatorViewer.TrackOnScreen = toggle.isOn;
    }

    public void ToggleOffScreen()
    {
        IndicatorViewer.TrackOffScreen = toggle.isOn;
    }

    public void ToggleTargetCamera()
    {
        IndicatorTargetCamera targetCam = currentTarget.GetComponent<IndicatorTargetCamera>();
        if (targetCam == null)
            targetCam = currentTarget.AddComponent<IndicatorTargetCamera>();
        targetCam.enabled = toggle.isOn;
    }

    public void ToggleDistanceTracker()
    {
        IndicatorDistanceTracker targetDistance = currentTarget.GetComponent<IndicatorDistanceTracker>();
        if (targetDistance == null)
            targetDistance = currentTarget.AddComponent<IndicatorDistanceTracker>();
        targetDistance.enabled = toggle.isOn;
    }

    public void ToggleOffScreenRotates()
    {
        viewer.OffScreenRotates = toggle.isOn;
    }

    public void ToggleTrackTarget()
    {
        if (toggle.isOn)
        {
            IndicatorViewer.TrackTarget(currentTarget);
            Debug.Log("Tracking target: " + currentTarget);
        }
        else
        {
            IndicatorViewer.UntrackTarget(currentTarget);
            Debug.Log("Untracking target: " + currentTarget);
        }
    }

    public void ToggleAutoScale()
    {   viewer.AutoScale = toggle.isOn; }

    public void UpdateScalingFactor()
    {   viewer.ScalingFactor = slider.value; }

    public void UpdateIndicatorEdgeDistance()
    {   viewer.IndicatorEdgeDistance = slider.value;    }

    public void UpdateTargetEdgeDistance()
    {
        viewer.TargetEdgeDistance = slider.value;
    }

    //  TARGET
    public void ToggleForceOnScreen()
    {
        if (currentTarget.GetComponent<IndicatorTarget>() != null)
            currentTarget.GetComponent<IndicatorTarget>().ForceOnScreen = toggle.isOn;
    }

    public void ToggleForceOffScreen()
    {
        if (currentTarget.GetComponent<IndicatorTarget>() != null)
            currentTarget.GetComponent<IndicatorTarget>().ForceOffScreen = toggle.isOn;
    }

    //  UTILITIES
    public void SetColor()
    {
        IndicatorColor color = currentTarget.GetComponent<IndicatorColor>();
        if (color == null)
            color = currentTarget.AddComponent<IndicatorColor>();
        color.ChangeColor(Color.red, true, true, true);
    }
}
