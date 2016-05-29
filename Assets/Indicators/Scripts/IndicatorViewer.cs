using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IndicatorViewer : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-Assigned Variables")]
    [Tooltip("The global default panel prefab that hold all the indicator UI for each target. Will automatically create a Canvas to house this panels.")]
    public GameObject DefaultIndicatorPanel;
    [Tooltip("The camera that will view the indicators. If left empty, will be assigned to the main camera instead.")]
    public Camera ViewerCamera;
    //[Tooltip("A transform in which indicator directions will be calculated from. If left emtpy, indicator directions will be calculated from this camera's position instead.")]
    //public Transform Viewer;

    //  Settings & options
    [Header("Settings")]
    [Tooltip("The sorting layer for all the UI. Lower value = behind UI. Higher value = front of UI")]
    public int CanvasSortingOrder = -100;
    [Tooltip("How many seconds before indicators update their tracking. Higher = better performance, but more stuttering")] [Range(0, 1)] 
    public float UpdateInterval = 0.015f;
    [Tooltip("The farthest distance indicators will reach from the top & buttom edges of the screen.")] [Range(0, 1)]
    public float EdgeHeightDistance = 0.9f;
    [Tooltip("The farthest distance indicators will reach from the left & right edges of the screen.")] [Range(0, 1)]
    public float EdgeWidthDistance = 0.9f;
    [Tooltip("Does the off-screen indicator rotate towards the target? Use for arrow-type indicators. False for portrait-style indicators")]
    public bool RotateTowardsTargetOffscreen = true;

    [Space(10)] [Tooltip("Should indicators track target when it is visable to the camera?")]
    public bool ShowOnVisable = true;
    [Tooltip("Should target indicator be disabled when target is too far from the viewer? Enabled = better performance")]
    public bool DisableOnDistance = true;
    [Tooltip("The max distance of the target from the viewer for the target's indicator to disable.")]
    public float DisableDistance = 100;

    [Space(10)] [Tooltip("Should indicators automatically scale based on the distance from the viewer?")]
    public bool AutoScale = true;
    [Tooltip("The minimum and maximum scaling size of the indicator.")]
    public float MinScaleSize = 0.2f;
    public float MaxScaleSize = 10;

    //  Info related
    [Header("Info")]
    [Tooltip("List containing all the targets currently being activly tracked. (Excludes inactive objects)")]
    public List<IndicatorTarget> IndicatorTargets = new List<IndicatorTarget>();

    //  Variables
    private GameObject indicatorCanvas;

    void Awake()
    {
        //  If no custom camera is assigned, just main camera
        if (ViewerCamera == null)
            ViewerCamera = Camera.main;  

        //  Create canvas is if doesnt already exsist
        if (indicatorCanvas == null)
            CreateIndicatorCanvas();
	}
	
    void Start()
    {
        //  start couroutine for updating indicators for every target.
        StartCoroutine(UpdateIndicators());
    }

    //  Updates each indicator in the Targets list on a delay interval instad of every frame. (adjustable performance)
    IEnumerator UpdateIndicators()
    {
        while(true)
        {
            //  Loop through each indicator in the target list...
            for (int i = 0; i < IndicatorTargets.Count; i++)
            {
                //  Update the target's indicator
                IndicatorTargets[i].UpdateIndicator(ViewerCamera);
            }
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    //  Create a default canvas for the indicator panels and set parameters.
    private void CreateIndicatorCanvas()
    {
        //  Create gameobject that holds canvas
        indicatorCanvas = new GameObject();
        indicatorCanvas.name = "Indicator_Canvas";
        indicatorCanvas.layer = 5;

        //  Create Canvas
        Canvas canvas = indicatorCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = CanvasSortingOrder;

        //  Create default components for canvas
        CanvasScaler cs = indicatorCanvas.AddComponent<CanvasScaler>();
        cs.scaleFactor = 1;
        cs.dynamicPixelsPerUnit = 10;
        indicatorCanvas.AddComponent<GraphicRaycaster>();
    }

    //  Getters/Setters
    public GameObject IndicatorCanvas
    {
        get { return indicatorCanvas; }
    }
}
