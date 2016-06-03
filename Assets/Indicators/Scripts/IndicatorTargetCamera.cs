using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IndicatorTargetCamera : MonoBehaviour
{
    //  Settings & options
    [Header("Settings")]
    [Tooltip("Offset position for the camera to see the target.")]
    public Vector3 CameraOffset = new Vector3(0, 0, -2);
    [Tooltip("Viewing size of the camera. Adjust to properly size target.")]
    public float CameraViewSize = 1.5f;
    [Tooltip("Farclipping plane of the camera. Adjust for long meshed objects.")]
    public int CameraViewDistance = 4;

    [Space(10)]
    [Tooltip("The texture size of the target. Lower = better performance")]
    public int TargetResolution = 128;
    [Tooltip("The layer that the camera will render. Only objects with this layer can be seen by the TargetCamera.")]
    public LayerMask TargetLayer = 1 << 0;

    //  private variables
    private IndicatorTarget _indicatorTarget;
    private IndicatorPanel _indicatorPanel;
    private GameObject targetCamGO;
    //private Camera targetCamera;
    private GameObject targetCameraImage;
    private RenderTexture renderTexture;

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
        targetCamGO = new GameObject("Indicator_TargetCam");
        //targetCamGO.transform.SetParent(transform);
        //targetCamGO.transform.position = new Vector3(0, 0, -2) + transform.position;

        //  Create the render texture & set parameters
        renderTexture = new RenderTexture(TargetResolution, TargetResolution, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        renderTexture.antiAliasing = 1;
        renderTexture.name = "TargetCamRenderTexture";
        renderTexture.Create();

        //  Create Camera and set up parameters
        Camera targetCamera = targetCamGO.AddComponent<Camera>();
        targetCamera.cullingMask = TargetLayer;
        targetCamera.orthographic = true;
        targetCamera.orthographicSize = CameraViewSize;
        targetCamera.farClipPlane = CameraViewDistance;
        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.targetTexture = renderTexture;

        //  Create the target camera raw image for the panel
        GameObject TargetCamImageGO = new GameObject("TargetCameraImage");
        TargetCamImageGO.layer = 1 << 4;
        RawImage rawImage = TargetCamImageGO.AddComponent<RawImage>();
        rawImage.raycastTarget = false;

        //  Set-up targetCamImage
        StartCoroutine(AssignRenderTexture(renderTexture, TargetCamImageGO));
    }

    void Update()
    {
        if (_indicatorPanel != null)
        {
            if (_indicatorTarget.IsVisable && _indicatorPanel.TargetCamImage != null && _indicatorPanel.OnScreenImage != null)
            {
                _indicatorTarget.IndicatorPanel.TargetCamImage.SetActive(false);
                targetCamGO.SetActive(false);
            }
            else if (!_indicatorTarget.IsVisable && _indicatorPanel.TargetCamImage != null && _indicatorPanel.OffScreenImage != null)
            {
                _indicatorTarget.IndicatorPanel.TargetCamImage.SetActive(true);
                targetCamGO.SetActive(true);

                //  Update camera position to target
                targetCamGO.transform.position = CameraOffset + transform.position;
            } 
        }
    }

    //  Using ienumerator because the indicator panel may not have been created yet thus we need to keep checking till it exist.
    IEnumerator AssignRenderTexture(RenderTexture renderTexture, GameObject targetCamImageGO)
    {
        //  Change color of all the indicator panel items
        _indicatorPanel = GetComponent<IndicatorTarget>().IndicatorPanel;

        while (_indicatorPanel == null)
        {
            _indicatorPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
            yield return null;
        }

        //  Now that the indicator panel exist, finish set-up
        targetCamImageGO.transform.SetParent(_indicatorTarget.IndicatorPanel.transform);
        targetCamImageGO.transform.localPosition = Vector3.zero;
        targetCamImageGO.transform.localScale = Vector3.one;
        _indicatorPanel.TargetCamImage = targetCamImageGO;
        _indicatorPanel.TargetCamImage.GetComponent<RawImage>().texture = renderTexture;
    }
}
