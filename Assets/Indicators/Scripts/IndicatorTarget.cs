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

    //  Settings & options
    [Header("Settings")]
    [Tooltip("Offset position of the on-screen indicator from the target. If left to 0, the indicator will be positioned at the center of the target.")]
    public Vector2 OnScreenIndicatorOffset;
    [Tooltip("Should indicators track target when it is visable to the camera? (Overrides viewer default) If false, the viewer default will be used instead")]
    public bool ShowOnVisable;

    //  Variables
    private IndicatorPanel indicatorPanel;
    private bool isVisable;
    float previousAxisPosition;             //   the last depth value of the scaling axis. (used for scaling the indicator)

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
            panel.name = name + " Panel";
            panel.transform.SetParent(_viewer.IndicatorCanvas.transform);
            indicatorPanel = panel.GetComponent<IndicatorPanel>();

            //  Set the offset position of the onscreen image.
            if (indicatorPanel.OnScreenImage != null)
                indicatorPanel.OnScreenImage.transform.position += new Vector3(OnScreenIndicatorOffset.x, OnScreenIndicatorOffset.y, 0);     

            previousAxisPosition = indicatorPanel.transform.position.z;     //  Assign the initial z position. (used for scaling the indicator)

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

            //  if the viewer allows indicators to show when the target is visable
            if ((ShowOnVisable || _viewer.ShowOnVisable) && indicatorPanel.OnScreenImage != null)
            {
                //  Get distance from this target and viewer
                float distanceFromViewer = GetDistance(transform.position, _viewer.transform.position);

                //  If the on-screen indicator is within distance threshold, then do stuff...  
                if (CheckDisableOnDistance(distanceFromViewer) == false)
                {
                    //  Enable the indicator image.
                    indicatorPanel.gameObject.SetActive(true);              //  Enable Panel
                    if (indicatorPanel.OffScreenImage != null)
                        indicatorPanel.OffScreenImage.SetActive(false);     //  Disable Off-Screen Image
                    indicatorPanel.OnScreenImage.SetActive(true);           //  Enable On-Screen Image

                    //  set the UI panel position to the target's position
                    indicatorPanel.transform.position = targetPosOnScreen;

                    //  If scaling is enabled && if the target's indicator axis towards the camera has changed...... then perform the update indicator method
                    if (_viewer.AutoScale && indicatorPanel.transform.position.z != previousAxisPosition)
                    {
                        UpdateScale(distanceFromViewer);

                        //  Record the new axis position of the indicator panel
                        previousAxisPosition = indicatorPanel.transform.position.z;
                    }
                }
                else
                    indicatorPanel.gameObject.SetActive(false);     //  Else if CheckDisableOnDistance() is true, Disable the indicator.
            }
            else
                indicatorPanel.gameObject.SetActive(false);     //  Else if ShowOnScreen is false, Disable the indicator.
        }
        //  Else, traget is Off-Screen...
        else
        {
            // Set target as invisable.
            isVisable = false;

            if (indicatorPanel.OffScreenImage != null)
            {
                //  Get distance from this target and viewer
                float distanceFromViewer = GetDistance(transform.position, _viewer.transform.position);

                //  If the off-screen indicator is within distance threshold, then do stuff...  
                if (CheckDisableOnDistance(distanceFromViewer) == false)
                {
                    //  Enable the indicator if it exist
                    indicatorPanel.gameObject.SetActive(true);              //  Enable Panel
                    indicatorPanel.OffScreenImage.SetActive(true);          //  Enable Off-Screen Image
                    if (indicatorPanel.OnScreenImage != null)
                        indicatorPanel.OnScreenImage.SetActive(false);      //  Disable On-Screen Image

                    //  Update the indicator positon and rotation based on the target's position on the screen
                    UpdateOffScreen(targetPosOnScreen);

                    //  If scaling is enabled... then perform the update indicator method
                    if (_viewer.AutoScale)
                        UpdateScale(distanceFromViewer);
                }
                else
                    indicatorPanel.gameObject.SetActive(false);     //  Else if CheckDisableOnDistance() is true, Disable the indicator.
            }
            else
                indicatorPanel.gameObject.SetActive(false);     //  Else if CheckDisableOnDistance() is true, Disable the indicator.
        }
    }

    //  Returns true if target is within the camera screen boundaries and it's near clipping plane.
    private bool OnScreen(Vector3 pos, Camera camera)
    {
        if (pos.x < Screen.width && pos.x > 0 && 
            pos.y < Screen.height && pos.y > 0 &&
            pos.z > camera.nearClipPlane)
            return true;
        return false;
    }

    //  Returns false if indicator does not need to be disabled because either DisableOnDistance is false or distance is within threshold. 
    private bool CheckDisableOnDistance(float distance)
    {
        //  Check if indicator will disable on distance and check if distance is over the distance threshold
        if (_viewer.DisableOnDistance && distance > _viewer.DisableDistance)
            return true;
        return false;
    }

    //  Updates the scaling of the UI based on distance provided
    private void UpdateScale(float distance)
    {
        //  Scale based on distance & set the scale based on distance
        indicatorPanel.transform.localScale = new Vector2(10 / distance, 10 / distance);

        //  If the indicator scale is lower then or equal to the minimum size... Then, set the scale to the minimum size.
        //  Else if the indicator scale is greater or equal to the maximum size... Then, set the scale to the maximum size.
        if (indicatorPanel.transform.localScale.x <= _viewer.MinScaleSize || indicatorPanel.transform.localScale.y <= _viewer.MinScaleSize)
            indicatorPanel.transform.localScale = new Vector2(_viewer.MinScaleSize, _viewer.MinScaleSize);
        else if (indicatorPanel.transform.localScale.x >= _viewer.MaxScaleSize || indicatorPanel.transform.localScale.y >= _viewer.MaxScaleSize)
            indicatorPanel.transform.localScale = new Vector2(_viewer.MaxScaleSize, _viewer.MaxScaleSize);

        //Debug.Log("Scaling...");
    }

    //  Updates the indicator based on the off-screen target
    private void UpdateOffScreen(Vector3 targetPosOnScreen)
    {
        //  Create a variable for the center position of the screen.
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

        //  Set newIndicatorPos anchor to the center of the screen instead of bottom left
        Vector3 newIndicatorPos = targetPosOnScreen - screenCenter;

        //  Flip the newIndicatorPos to correct the calculations for indicators behind the camera.
        if (newIndicatorPos.z < 0)
            newIndicatorPos *= -1;

        //  Get angle from center of screen to target position
        float angle = Mathf.Atan2(newIndicatorPos.y, newIndicatorPos.x);
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
            newIndicatorPos = new Vector2(-screenBounds.y / m, screenBounds.y);
        else
            newIndicatorPos = new Vector2(screenBounds.y / m, -screenBounds.y);

        //  Check left & right
        if (newIndicatorPos.x > screenBounds.x)
            newIndicatorPos = new Vector2(screenBounds.x, -screenBounds.x * m);
        else if (newIndicatorPos.x < -screenBounds.x)
            newIndicatorPos = new Vector2(-screenBounds.x, screenBounds.x * m);

        //  Reset the newIndicatorPos anchor back to bottom left corner.
        newIndicatorPos += screenCenter;

        //  Assign its new position and rotation
        indicatorPanel.transform.position = new Vector3(newIndicatorPos.x, newIndicatorPos.y, targetPosOnScreen.z);
        if (_viewer.RotateTowardsTargetOffscreen)
            indicatorPanel.OffScreenImage.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        else
            indicatorPanel.OffScreenImage.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    //  Returns the distance magnitude between two vector3 positions. (Faster then built-in Vector3.Distance function)
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
