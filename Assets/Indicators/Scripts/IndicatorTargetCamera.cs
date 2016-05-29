using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorTargetCamera : MonoBehaviour
{
    //  User-assigned variables
    public Vector3 CameraOffset = new Vector3(0, 0, -2);

    private IndicatorTarget _indicatorTarget;
    private GameObject targetCamGO;
    private Camera targetCam;

    void Awake()
    {
        //  Find and assin references
        _indicatorTarget = GetComponent<IndicatorTarget>();
    }

	// Use this for initialization
	void Start ()
    {
        //  Create the camera
        CreateTargetCamera();
    }
	
    //  Creates the target camera that follows the target and 
    private void CreateTargetCamera()
    {
        //  Create empty gameobject to hold the camera
        targetCamGO = new GameObject();
        targetCamGO.name = "Indicator_TargetCam";
        //targetCamGO.transform.SetParent(transform);
        //targetCamGO.transform.position = new Vector3(0, 0, -2) + transform.position;

        //  Create the render texture & set parameters
        RenderTexture renderTexture = new RenderTexture(128, 256, 16, RenderTextureFormat.ARGB32);
        renderTexture.name = "TargetCamRenderTexture";
        renderTexture.Create();
        StartCoroutine(AssignRenderTexture(renderTexture));

        //  Create Camera and set up parameters
        targetCam = targetCamGO.AddComponent<Camera>();
        targetCam.orthographic = true;
        targetCam.orthographicSize = 1;
        targetCam.farClipPlane = 4;
        targetCam.clearFlags = CameraClearFlags.Depth;
        targetCam.targetTexture = renderTexture;

    }

    void Update()
    {
        if (_indicatorTarget.IsVisable && targetCam != null)
        {
            _indicatorTarget.IndicatorPanel.TargetCamImage.SetActive(false);
            targetCam.enabled = false;
        }
        else if (!_indicatorTarget.IsVisable && targetCam != null)
        {
            _indicatorTarget.IndicatorPanel.TargetCamImage.SetActive(true);
            targetCam.enabled = true;

            //  Update camera position to target
            targetCamGO.transform.position = CameraOffset + transform.position;
        } 
    }

    //  Using ienumerator because the indicator panel may not have been created yet thus we need to keep checking till it exist.
    IEnumerator AssignRenderTexture(RenderTexture renderTexture)
    {
        //  Change color of all the indicator panel items
        IndicatorPanel IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;

        while (IPanel == null)
        {
            IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
            yield return null;
        }

        IPanel.TargetCamImage.GetComponent<RawImage>().texture = renderTexture;
    }
}
