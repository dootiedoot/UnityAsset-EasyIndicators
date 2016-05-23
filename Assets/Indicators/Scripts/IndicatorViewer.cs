using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class IndicatorViewer : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-Assigned Variables")]
    [Tooltip("The Canvas that will hold all the indicator UI Panels. If left empty, a Unity default Canvas will automatically be created instead.")]
    public GameObject DefaultIndicatorCanvas;
    [Tooltip("The global default indicator UI Panel shown on all targets. [REQUIRED]")]
    public GameObject DefaultIndicatorPanel;

    //  Settings & options
    [Header("Settings")]
    [Tooltip("Should indicators track target when it is visable to the camera?")]
    public bool ShowOnVisable;
    [Tooltip("How many seconds before indicators update their tracking. (Default: 0.01) (Higher = better performance, but more stuttering)")] [Range(0, 1)] 
    public float UpdateInterval = 0.01f;
    [Tooltip("The farthest distance indicators will reach from the top & buttom edges of the screen.")] [Range(0, 1)]
    public float EdgeHeightDistance;
    [Tooltip("The farthest distance indicators will reach from the left & right edges of the screen.")] [Range(0, 1)]
    public float EdgeWidthDistance;

    //  Info related
    //  All the targets currently being tracked.
    public List<IndicatorTarget> IndicatorTargets = new List<IndicatorTarget>();

    // Private variables
    private Camera viewerCamera;

    void Awake()
    {
        viewerCamera = GetComponent<Camera>();

        if (DefaultIndicatorCanvas == null)
            CreateIndicatorCanvas();   
	}
	
    void Start()
    {
        StartCoroutine(UpdateIndicators());
    }

    //  Updates each indicator in the Targets list on a delay interval instad of every frame. (saves processing power)
    IEnumerator UpdateIndicators()
    {
        while(true)
        {
            for (int i = 0; i < IndicatorTargets.Count; i++)
            {
                IndicatorTargets[i].UpdateIndicator(viewerCamera);
            }
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    //  Sets up a default canvas for the indicator UI if it doesnt exsists.
    private void CreateIndicatorCanvas()
    {
        DefaultIndicatorCanvas = new GameObject();
        DefaultIndicatorCanvas.name = "Indicator_Canvas";
        DefaultIndicatorCanvas.layer = 5;
        Canvas canvas = DefaultIndicatorCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler cs = DefaultIndicatorCanvas.AddComponent<CanvasScaler>();
        cs.scaleFactor = 1;
        cs.dynamicPixelsPerUnit = 10;
        DefaultIndicatorCanvas.AddComponent<GraphicRaycaster>();
    }
}
