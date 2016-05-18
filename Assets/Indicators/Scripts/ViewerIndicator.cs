using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class ViewerIndicator : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-assigned variables")]
    [Tooltip("The Canvas that will hold all the indicator UI Panels. If left empty, a Unity default Canvas will be created instead.")]
    public GameObject DefaultIndicatorCanvas;
    [Tooltip("The global default indicator UI Panel shown on all targets.")]
    public GameObject DefaultIndicatorPanel;

    //  Settings & options
    [Header("Settings")]
    [Tooltip("Should indicators track target when it is visable to the camera?")]
    public bool ShowOnVisable;
    [Tooltip("How often the indicators update their tracking. (Higher = Better performance, but more stuttering)")] [Range(0, 1)] 
    public float UpdateInterval = 0.01f;
    [Tooltip("The farthest distance indicators will reach from the edges of the screen.")] [Range(0, 1)]
    public float DistanceFromEdge;

    //  Info related
    [Header("Info")]
    [Tooltip("All the targets currently being tracked.")]
    public List<TargetIndicator> Targets = new List<TargetIndicator>();

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
            for (int i = 0; i < Targets.Count; i++)
            {
                //  If the inidicator doesn't exsist, remove it from the list and destory the indicator gameobject. (When target has been destroyed)
                if (Targets[i] == null)
                {
                    Destroy(Targets[i].GetIndicatorPanel());
                    Targets.Remove(Targets[i]);
                }

                Targets[i].UpdateIndicator(viewerCamera);
            }
            yield return new WaitForSeconds(UpdateInterval);
        }
    }

    //  Sets up a default canvas for the indicator UI if it doesnt exsists.
    private void CreateIndicatorCanvas()
    {
        DefaultIndicatorCanvas = new GameObject();
        DefaultIndicatorCanvas.name = "Canvas_EasyIndicators";
        DefaultIndicatorCanvas.layer = 5;
        Canvas canvas = DefaultIndicatorCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler cs = DefaultIndicatorCanvas.AddComponent<CanvasScaler>();
        cs.scaleFactor = 1;
        cs.dynamicPixelsPerUnit = 10;
        DefaultIndicatorCanvas.AddComponent<GraphicRaycaster>();
    }
}
