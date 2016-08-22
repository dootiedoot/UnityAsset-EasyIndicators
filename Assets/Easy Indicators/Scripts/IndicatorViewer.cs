using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(IndicatorTarget))]
public class IndicatorViewer : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-Assigned Variables")]
    [Tooltip("The global default panel prefab that hold all the indicator UI for each target. Will automatically create a Canvas to house this panels.")]
    public GameObject DefaultIndicatorPanel;
    [Tooltip("The camera that will view the indicators. If left empty, will be assigned to the main camera instead.")]
    public Camera ViewerCamera;
    [Tooltip("The gameobject that the indicator's distance calculations will be based from. If left empty, it will be assigned to this gameobject instead. NOTE: ViewerObject should move with the ViewerCamera.")]
    public GameObject ViewerObject;

    //  Settings & options
    [Header("Settings")]
    [Tooltip("The sorting layer for all the indicators. Lower value = behind UI. Higher value = front of UI")]
    public int CanvasSortingOrder = -100;
    [Tooltip("How many seconds before indicators update their tracking. Higher = better performance, but more stuttering")] [Range(0, 1)] 
    public float UpdateInterval = 0.015f;
    [Tooltip("The farthest distance indicators will reach from the screen center to the screen edges. Align slider with TargetEdgeDistance for seamless transition.")] [Range(0, 1)]
    public float IndicatorEdgeDistance = 0.9f;
    [Tooltip("The farthest distance the target needs to reach from the screen center to the screen edges to be considered off-screen. Align slider with IndicatorEdgeDistance for seamless transition.")] [Range(0.5f, 1)]
    public float TargetEdgeDistance = 0.95f;

    [Space(10)]
    [Tooltip("Should target indicator be disabled when target is too far from the viewer? Enabled = better performance")]
    public bool DisableOnDistance = true;
    [Tooltip("The max distance of the target from the viewer for the target's indicator to disable.")]
    public float DisableDistance = 100;

    [Space(10)]
    [Tooltip("Does the off-screen indicator rotate towards the target? Set True for arrow-type indicators. Set False for portrait-style indicators.")]
    public bool OffScreenRotates = true;
    [Tooltip("Should indicators automatically scale based on the distance from the viewer?")]
    public bool AutoScale = true;
    public float ScalingFactor = 10;
    [Tooltip("The minimum and maximum X and Y scale size of the indicator.")]
    public float MinScaleSize = 0.2f;
    public float MaxScaleSize = 100;

    [Space(10)]
    [Tooltip("The duration of the transition in seconds.")]
    public float TransitionDuration = 0.25f;
    public Transitions OnScreenEnableTransition = Transitions.Scale;
    public Transitions OnScreenDisableTransition = Transitions.Scale;
    public Transitions OffScreenEnableTransition = Transitions.Scale;
    public Transitions OffScreenDisableTransition = Transitions.Scale;
    public enum Transitions
    { None, Slide, Fade, Rotate, RotateReverse, Scale}

    //  Private Variables
    private GameObject indicatorCanvas;
    private static bool isTracking = true;
    private static bool trackOnScreen = true;
    private static bool trackOffScreen = true;
    public static List<IndicatorTarget> Targets = new List<IndicatorTarget>();

    void Awake()
    {
        //  If no custom camera is assigned, use main camera
        if (ViewerCamera == null)
            ViewerCamera = Camera.main;

        //  If no viewer is assigned, use this gameobject
        if (ViewerObject == null)
            ViewerObject = gameObject;  

        //  Create canvas is if doesnt already exsist
        if (indicatorCanvas == null)
            CreateIndicatorCanvas();
	}

    void OnEnable()
    {
        //  start couroutine for updating indicators for every target.
        StartCoroutine(UpdateIndicators());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    #region Updates each indicator in the Targets list on a delay interval instad of every frame. (adjustable performance)
    IEnumerator UpdateIndicators()
    {
        //  Using while loop with delay
        while (true)
        {
            if (isTracking)
            {
                //Debug.Log("Tracking: " + IndicatorTargets.Count);

                //  Loop through each indicator in the target list...
                for (int i = 0; i < Targets.Count; i++)
                {
                    //  Update the target's indicator
                    Targets[i].UpdateIndicator();
                }
            }

            yield return new WaitForSeconds(UpdateInterval);
        }
    }
    #endregion

    #region Create the indicator canvas
    //  Create a default canvas for the indicator panels and set parameters.
    private void CreateIndicatorCanvas()
    {
        //  Create gameobject that holds canvas
        indicatorCanvas = new GameObject("Indicator_Canvas");
        indicatorCanvas.layer = 1 << 4;

        //  Create Canvas
        Canvas canvas = indicatorCanvas.AddComponent<Canvas>();
        //canvas.renderMode = RenderMode.WorldSpace;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = CanvasSortingOrder;

        //  Create default components for canvas
        CanvasScaler cs = indicatorCanvas.AddComponent<CanvasScaler>();
        cs.scaleFactor = 1;
        cs.dynamicPixelsPerUnit = 10;
        indicatorCanvas.AddComponent<GraphicRaycaster>();
    }
    #endregion

    #region public set/get methods
    //  Tracks target; add a IndicatorTarget component to the target if it doesn't already exist and add to tracking list.
    public static void TrackTarget(GameObject target)
    {
        IndicatorTarget ITarget = target.GetComponent<IndicatorTarget>();
        //  If the target doesn't have a indicator... Add one
        if (ITarget == null)
            ITarget = target.AddComponent<IndicatorTarget>();
        //  Else if the target already has an indicator, check if it just needs to be added to the tracking list if not already
        else if (!Targets.Contains(ITarget))
            Targets.Add(ITarget);
        else
            Debug.Log("Target is already being tracked.");
        ITarget.enabled = true;
    }

    //  Untracks target; removes target from tracking list
    public static void UntrackTarget(GameObject target)
    {
        IndicatorTarget ITarget = target.GetComponent<IndicatorTarget>();
        ITarget.enabled = false;
        Targets.Remove(ITarget);
    }

    //  Returns the IndicatorTarget component of the target
    public static IndicatorTarget GetIndicatorTarget(GameObject target)
    {
        IndicatorTarget ITarget = target.GetComponent<IndicatorTarget>();
        if (ITarget != null)
            for (int i = 0; i < Targets.Count; i++)
                if (ITarget = Targets[i])
                    return ITarget;
        return null;
    }

    public static void SetTracking(bool trackOnScreen, bool trackOffScreen)
    {
        TrackOnScreen = trackOnScreen;
        TrackOffScreen = trackOffScreen;
    }

    public static void StartTracking()
    {   isTracking = true;    }

    public static void StopTracking()
    {   isTracking = false;   }

    public static bool IsTracking
    {   get { return isTracking; } }

    public static bool TrackOnScreen
    {
        get { return trackOnScreen; }
        set { trackOnScreen = value; }
    }

    public static bool TrackOffScreen
    {
        get { return trackOffScreen; }
        set { trackOffScreen = value; }
    }

    public GameObject IndicatorCanvas
    {   get { return indicatorCanvas; } }
    #endregion
}
