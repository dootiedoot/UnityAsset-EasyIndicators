using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorTargetCamera : MonoBehaviour
{

    private IndicatorTarget _indicatorTarget;
    private Camera targetCam;

    void Awake()
    {
        _indicatorTarget = GetComponent<IndicatorTarget>();
    }

	// Use this for initialization
	void Start ()
    {
        CreateTargetCamera();
    }
	
    //  Creates the target camera that follows the target and 
    private void CreateTargetCamera()
    {
        //  Create empty gameobject to hold the camera
        GameObject TargetCamGO = new GameObject();
        TargetCamGO.name = "Indicator_TargetCam";
        TargetCamGO.transform.SetParent(transform);
        TargetCamGO.transform.localPosition = new Vector3(0, 0, -2);

        //  Create the render texture & set parameters
        RenderTexture renderTexture = new RenderTexture(128, 256, 16, RenderTextureFormat.ARGB32);
        renderTexture.name = "TargetCamRenderTexture";
        renderTexture.Create();
        _indicatorTarget.IndicatorPanel.TargetCamImage.GetComponent<RawImage>().texture = renderTexture;

        //  Create Camera and set up parameters
        targetCam = TargetCamGO.AddComponent<Camera>();
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
        } 
    }
}
