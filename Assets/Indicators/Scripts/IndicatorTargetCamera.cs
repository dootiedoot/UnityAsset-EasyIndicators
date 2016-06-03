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
        //  Set-up targetCamImage
        StartCoroutine(AssignRenderTexture());
    }
	
    //  Creates the target camera, target's RenderTexture, and set-up UI stuff 
    private void CreateTargetCamera()
    {
        //  1. Create empty gameobject to hold the camera
        targetCamGO = new GameObject("Indicator_TargetCam");
        //targetCamGO.transform.SetParent(transform);
        //targetCamGO.transform.position = CameraOffset + transform.position;

        //  2. Create the target camera raw image for the panel
        GameObject TargetCamImageGO = new GameObject("TargetCameraImage");
        TargetCamImageGO.layer = 1 << 4;
        TargetCamImageGO.transform.SetParent(_indicatorTarget.IndicatorPanel.transform);
        TargetCamImageGO.transform.localPosition = Vector3.zero;
        TargetCamImageGO.transform.localScale = Vector3.one;
        _indicatorPanel.TargetCamImage = TargetCamImageGO;
        RawImage rawImage = TargetCamImageGO.AddComponent<RawImage>();
        rawImage.raycastTarget = false;

        //  3. Create the render texture & set parameters
        //renderTexture = new RenderTexture(TargetResolution, TargetResolution, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        //renderTexture.antiAliasing = 1;
        renderTexture = RenderTexture.GetTemporary(TargetResolution, TargetResolution, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);
        renderTexture.name = "TargetCamRenderTexture";
        _indicatorPanel.TargetCamImage.GetComponent<RawImage>().texture = renderTexture;

        //  4. Create Camera and set up parameters
        Camera targetCamera = targetCamGO.AddComponent<Camera>();
        targetCamera.cullingMask = TargetLayer;
        targetCamera.orthographic = true;
        targetCamera.orthographicSize = CameraViewSize;
        targetCamera.farClipPlane = CameraViewDistance;
        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.targetTexture = renderTexture;
    }

    void LateUpdate()
    {
        if (_indicatorPanel != null && _indicatorPanel.TargetCamImage != null)
        {
            if (_indicatorTarget.IsVisable && _indicatorPanel.OnScreenImage != null)
            {
                //  Disable Camera
                _indicatorTarget.IndicatorPanel.TargetCamImage.SetActive(false);
                targetCamGO.SetActive(false);
            }
            else if (!_indicatorTarget.IsVisable && _indicatorPanel.OffScreenImage != null)
            {
                //  Enable Camera
                _indicatorTarget.IndicatorPanel.TargetCamImage.SetActive(true);
                targetCamGO.SetActive(true);

                //  Update camera position/rotation to target
                targetCamGO.transform.position = CameraOffset + transform.position;
                //targetCamGO.transform.rotation = Quaternion.identity;
            } 
        }
    }

    //  Using ienumerator because the indicator panel may not have been created yet thus we need to keep checking till it exist.
    IEnumerator AssignRenderTexture()
    {
        //  Change color of all the indicator panel items
        _indicatorPanel = GetComponent<IndicatorTarget>().IndicatorPanel;

        while (_indicatorPanel == null)
        {
            _indicatorPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
            yield return null;
        }

        //  Now that the indicator panel exist, create the camera
        CreateTargetCamera();
    }
}
