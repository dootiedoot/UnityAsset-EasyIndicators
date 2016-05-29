using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class IndicatorTarget : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-Assigned Variables")]
    [Tooltip("The custom indicator Panel shown only on this target. (Overrides viewer default) If left empty, the viewer default will be used instead.")]
    public GameObject CustomIndicatorPanel;
    [Tooltip("Offset position of the on-screen indicator from the target. If left to 0, the indicator will be positioned at the center of the target.")]
    public Vector2 OnScreenIndicatorOffset;
    [Tooltip("Should indicators track target when it is visable to the camera? (Overrides viewer default) If false, the viewer default will be used instead")]
    public bool ShowOnVisable;

    //  Variables
    private IndicatorPanel indicatorPanel;
    private bool isVisable;
    float previousAxisPosition;             //   the last depth value of the scaling axis
    //bool isVisable { get; set; }

    //  References
    private IndicatorViewer _viewer;

    void Awake()
    {
        //  Find and assign references
        _viewer = FindObjectOfType<IndicatorViewer>();
    }

    void Start()
    {
        //  Create the indicator for this target
        InitializeIndicator();
    }

    //  OnEnable/OnDisable used for pooling objects
    void OnEnable()
    {
        //  Add the target the the viewer's target list so viewer can track this target.
        if (!_viewer.IndicatorTargets.Contains(this) && indicatorPanel != null)
            _viewer.IndicatorTargets.Add(this);
        //  Disable the panel so viewer does not see it ingame
        if (indicatorPanel != null)
            indicatorPanel.gameObject.SetActive(true);
    }
    void OnDisable()
    {
        //  Remove the target from the viewer's list so viewer no longer tracks this target.
        _viewer.IndicatorTargets.Remove(this);
        //  Enable the panel so viewer can see it ingame
        if (indicatorPanel != null)
            indicatorPanel.gameObject.SetActive(false);
    }

    //  Create & set-up the indicator panel
    private void InitializeIndicator()
    {
        //  If no custom indicator panel is assigned to this gameobject, the viewer default indicator panel will be used instead
        if (CustomIndicatorPanel == null)
            CustomIndicatorPanel = _viewer.DefaultIndicatorPanel;

        //  If customIndicatorPanel is still null because there is no viewer default, throw error 
        try
        {
            //  Create the runtime indicator object and assign it under the canvas. 
            GameObject panel = Instantiate(CustomIndicatorPanel, Vector2.zero, Quaternion.identity) as GameObject;
            panel.transform.SetParent(_viewer.IndicatorCanvas.transform);
            indicatorPanel = panel.GetComponent<IndicatorPanel>();
            indicatorPanel.OnScreenImage.transform.position += new Vector3(OnScreenIndicatorOffset.x, OnScreenIndicatorOffset.y, 0);     //  Set the offset position of the onscreen image.

            previousAxisPosition = indicatorPanel.transform.position.z;     //  Assign the initial axis position

            //  Add this target to the list of targets if not already
            if (!_viewer.IndicatorTargets.Contains(this))
              _viewer.IndicatorTargets.Add(this);
        }
        catch
        {
            Debug.LogError("The 'DefaultIndicatorPanel' requires a UnityUI Panel Gameobject!", _viewer);
        }
    }

    //  Performs the calculations to update the position & rotation of the indicator panel of this target.
    //  Called from 'IndicatorViewer' script.
    public void UpdateIndicator(Camera ViewerCamera)
    {
        //  Get the target's world position in screen coordinate position
        Vector3 targetPosOnScreen = ViewerCamera.WorldToScreenPoint(transform.position);

        //  if the target is visable on screen...
        if (OnScreen(targetPosOnScreen, ViewerCamera))
        {
            //  Set target to visable.
            isVisable = true;

            //  if the viewer allows indicators to show when the target is visable & within indicator distance 
            if (ShowOnVisable || _viewer.ShowOnVisable)
            {
                
                //  Boolean to determine where or not indicator should be enabled/disabled.
                bool EnableOnScreen = true;

                //  Get distance from this target and viewer
                float distanceFromViewer = GetDistance(transform.position, _viewer.transform.position);
                
                //  Check if indicator will disable on distance and check if distance is over the distance threshold
                if (_viewer.DisableOnDistance && distanceFromViewer > _viewer.DisableDistance)
                    EnableOnScreen = false;    

                //  If the on-screen indicator can be enabled, then do stuff...  
                if (EnableOnScreen)
                {
                    //  Enable the indicator image.
                    indicatorPanel.gameObject.SetActive(true);
                    if (indicatorPanel.OffScreenImage != null)
                        indicatorPanel.OffScreenImage.SetActive(false);
                    if (indicatorPanel.OnScreenImage != null)
                        indicatorPanel.OnScreenImage.SetActive(true);

                    //  set the UI panel position to the target's position
                    indicatorPanel.transform.position = targetPosOnScreen;

                    //  If scaling is enabled...
                    if (_viewer.ScaleOnDistance)
                        UpdateScale(distanceFromViewer);
                }
                else
                {
                    //  Disable the indicator image and set it as invisable.
                    indicatorPanel.gameObject.SetActive(false);
                }
            }
            else
            {
                //  Disable the indicator image and set it as invisable.
                indicatorPanel.gameObject.SetActive(false);
            }

               
        }
        //  Else, traget is Off-Screen...
        else
        {
            // Set target as invisable.
            isVisable = false;

            //  Enable the indicator image
            indicatorPanel.gameObject.SetActive(true);
            if (indicatorPanel.OffScreenImage != null)
                indicatorPanel.OffScreenImage.SetActive(true);
            if (indicatorPanel.OnScreenImage != null)
                indicatorPanel.OnScreenImage.SetActive(false);

            //  Create a variable for the center position of the screen.
            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

            //  Set targetPosOnScreen anchor to the center of the screen instead of bottom left
            targetPosOnScreen -= screenCenter;

            //  Flip the targetPosOnScreen to correct the calculations for indicators behind the camera.
            if (targetPosOnScreen.z < 0)
                targetPosOnScreen *= -1;
            
            //  Get angle from center of screen to target position
            float angle = Mathf.Atan2(targetPosOnScreen.y, targetPosOnScreen.x);
            angle -= 90 * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);    
            float sin = Mathf.Sin(angle);

            //targetPosOnScreen = screenCenter + new Vector3(sin * 150, cos * 150, 0);

            //  y = mx + b (intercept forumla)
            float m = cos / sin;

            //  Create the screen boundaries that the indicators reside in.
            Vector3 screenBounds = new Vector3(screenCenter.x * _viewer.EdgeWidthDistance, screenCenter.y * _viewer.EdgeHeightDistance);

            //  Check which screen side the target is currently in.
            //  Check top & bottom
            if (cos > 0)
                targetPosOnScreen = new Vector3(-screenBounds.y / m, screenBounds.y, 0);
            else
                targetPosOnScreen = new Vector3(screenBounds.y / m, -screenBounds.y, 0);

            //  Check left & right
            if (targetPosOnScreen.x > screenBounds.x)
                targetPosOnScreen = new Vector3(screenBounds.x, -screenBounds.x * m, 0);
            else if (targetPosOnScreen.x < -screenBounds.x)
                targetPosOnScreen = new Vector3(-screenBounds.x, screenBounds.x * m, 0);

            //  Reset the targetPosOnScreen anchor back to bottom left corner.
            targetPosOnScreen += screenCenter;

            //  Assign its new position and rotation
            indicatorPanel.transform.position = targetPosOnScreen;
            if (indicatorPanel.OffScreenImage != null)
                indicatorPanel.OffScreenImage.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

            //  Scaling
            indicatorPanel.transform.localScale = new Vector2(1, 1);
        }
    }

    //  Returns true if target is within the camera screen boundaries including the camera's clipping planes.
    private bool OnScreen(Vector3 pos, Camera cam)
    {
        if (pos.x < Screen.width && pos.x > 0 && 
            pos.y < Screen.height && pos.y > 0 &&
            pos.z > cam.nearClipPlane && pos.z < cam.farClipPlane)
            return true;
        return false;
    }

    //  Returns the distance between two vector3 positions. (Faster then built-in distance function)
    private float GetDistance(Vector3 PosA, Vector3 PosB)
    {
        Vector3 heading;

        //  Subtracting from both vectors returns the magnitude
        heading.x = PosA.x - PosB.x;
        heading.y = PosA.y - PosB.y;
        heading.z = PosA.z - PosB.z;

        //  Return the sqaure root of the sum of each sqaured vector axises.
        return Mathf.Sqrt((heading.x * heading.x) + (heading.y * heading.y) + (heading.z * heading.z));
    }

    private void UpdateScale(float distance)
    {
        //  If the target's indicator axis towards the camera has changed...
        if (indicatorPanel.transform.position.z != previousAxisPosition)
        {
            //  Scale based on distance & set the scale based on distance
            indicatorPanel.transform.localScale = new Vector2(10 / distance, 10 / distance);

            //  If the indicator scale is lower then or equal to the minimum size... Then, set the scale to the minimum size.
            //  Else if the indicator scale is greater or equal to the maximum size... Then, set the scale to the maximum size.
            if (indicatorPanel.transform.localScale.x <= _viewer.MinScaleSize || indicatorPanel.transform.localScale.y <= _viewer.MinScaleSize)
                indicatorPanel.transform.localScale = new Vector2(_viewer.MinScaleSize, _viewer.MinScaleSize);
            else if (indicatorPanel.transform.localScale.x >= _viewer.MaxScaleSize || indicatorPanel.transform.localScale.y >= _viewer.MaxScaleSize)
                indicatorPanel.transform.localScale = new Vector2(_viewer.MaxScaleSize, _viewer.MaxScaleSize);

            //  Record the new axis position of the indicator panel
            previousAxisPosition = indicatorPanel.transform.position.z;

            //Debug.Log(distance + " : " + indicatorPanel.transform.localScale);
        }
    }

    //  Getters/Setters
    public IndicatorPanel IndicatorPanel
    {
        get { return indicatorPanel; }
    }
    public bool IsVisable
    {
        get { return isVisable; }
        //set { isVisable = value; }
    }
}
