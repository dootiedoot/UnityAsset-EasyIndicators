using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(IndicatorTarget))]
public class IndicatorTargetCamera : MonoBehaviour
{
    //  Settings & options
    [Header("Settings")]
    [Tooltip("Offset position for the camera to see the target.")]
    public Vector3 CameraPositionOffset = new Vector3(0, 0, -2);
    [Tooltip("Offset rotation for the camera to see the target.")]
    public Vector3 CameraRotationOffset;
    [Tooltip("Farclipping plane of the camera. Adjust for long meshed objects.")]
    public int CameraFarclipDistance = 4;
    [Tooltip("Viewing size of the camera. Adjust to properly size target.")]
    public float CameraViewSize = 1.5f;

    [Space(10)]
    [Tooltip("The texture size of the target. Lower = better performance")]
    public int TargetResolution = 128;
    [Tooltip("The layer that the camera will render. Only objects with this layer can be seen by the TargetCamera.")]
    public LayerMask TargetLayer = 1 << 0;

    //  private variables
    private IndicatorTarget ITarget;
    private IndicatorPanel IPanel;
    private GameObject targetCamGO;
    private GameObject targetCameraIndicator;

    void Awake()
    {
        //  Find and assin references
        ITarget = GetComponent<IndicatorTarget>();
    }

	// Use this for initialization
	void Start ()
    {
        //  Set-up targetCamImage
        StartCoroutine(CoCreateTargetCamera());
    }

    //  Set active/inactive for target camera & the target camera indicator when script is enabled/disabled
    void OnEnable()
    {
        if (targetCameraIndicator != null)
            targetCameraIndicator.SetActive(true);
        if (targetCamGO != null)
            targetCamGO.SetActive(true);
    }
    void OnDisable()
    {
        if (targetCameraIndicator != null)
            targetCameraIndicator.SetActive(false);
        if (targetCamGO != null)
            targetCamGO.SetActive(false);
    }

    void LateUpdate()
    {
        if (IPanel != null && targetCamGO != null)
        {
            if (ITarget.IsVisable && IPanel.OnScreen != null)
            {
                //  Disable Camera
                //targetCameraIndicator.SetActive(false);
                targetCamGO.SetActive(false);
            }
            else if (!ITarget.IsVisable && IPanel.OffScreen != null)
            {
                //  Enable Camera
                //targetCameraIndicator.SetActive(true);
                targetCamGO.SetActive(true);

                //  Update camera position/rotation to target
                targetCamGO.transform.position = CameraPositionOffset + transform.position;
                targetCamGO.transform.rotation = Quaternion.Euler(CameraRotationOffset.x, CameraRotationOffset.y, CameraRotationOffset.z);
                targetCameraIndicator.transform.rotation = Quaternion.identity;
            } 
        }
    }

    #region IEnumerator that checks for a indicator panel that may not have been created yet thus we need to keep checking till it exist.
    IEnumerator CoCreateTargetCamera()
    {
        IPanel = ITarget.IndicatorPanel;

        while (IPanel == null)
        {
            IPanel = ITarget.IndicatorPanel;
            yield return null;
        }

        //  Now that the indicator panel exist, create the camera
        CreateTargetCamera();
    }
    #endregion

    #region Create the target camera and indicator camera
    //  Creates the target camera, target's RenderTexture, and set-up UI stuff 
    private void CreateTargetCamera()
    {
        //  1. Create empty gameobject to hold the camera
        targetCamGO = new GameObject("Indicator_TargetCam");
        targetCamGO.transform.SetParent(transform);
        targetCamGO.transform.position = CameraPositionOffset + transform.position;
        targetCamGO.transform.rotation = Quaternion.Euler(CameraRotationOffset.x, CameraRotationOffset.y, CameraRotationOffset.z);

        //  2. Create the target camera raw image for the panel
        targetCameraIndicator = new GameObject("TargetCameraImage");
        targetCameraIndicator.layer = 1 << 4;
        targetCameraIndicator.transform.SetParent(ITarget.IndicatorPanel.OffScreen.transform);
        targetCameraIndicator.transform.localPosition = Vector3.zero;
        targetCameraIndicator.transform.localScale = Vector3.one;
        //_indicatorPanel.TargetCam = targetCameraIndicator;
        RawImage rawImage = targetCameraIndicator.AddComponent<RawImage>();
        rawImage.raycastTarget = false;

        //  3. Create the render texture & set parameters
        //renderTexture = new RenderTexture(TargetResolution, TargetResolution, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        //renderTexture.antiAliasing = 1;
        RenderTexture renderTexture = RenderTexture.GetTemporary(TargetResolution, TargetResolution, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);
        renderTexture.name = "TargetCamRenderTexture";
        targetCameraIndicator.GetComponent<RawImage>().texture = renderTexture;

        //  4. Create Camera and set up parameters
        Camera targetCamera = targetCamGO.AddComponent<Camera>();
        targetCamera.cullingMask = TargetLayer;
        targetCamera.orthographic = true;
        targetCamera.orthographicSize = CameraViewSize;
        targetCamera.farClipPlane = CameraFarclipDistance;
        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = Color.clear;
        targetCamera.targetTexture = renderTexture;
    }
    #endregion
}
