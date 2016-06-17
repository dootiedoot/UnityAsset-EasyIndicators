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
    [Tooltip("Should indicators track target when it is On-Screen regardless of viewer default?")]
    public bool forceOnScreen = false;
    [Tooltip("Should indicators track target when it is Off-Screen regardless of viewer default?")]
    public bool forceOffScreen = false;

    //  Variables
    private IndicatorViewer _viewer;
    private IndicatorPanel IPanel;
    private bool isVisable;
    private bool isOnScreenEnabled;
    private bool isOffScreenEnabled;
    private float previousZposition;             //  the last depth value of the scaling axis. (used for scaling the indicator)
    private float distanceFromViewer;

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
        if (!IndicatorViewer.IndicatorTargets.Contains(this) && IPanel != null)
            IndicatorViewer.IndicatorTargets.Add(this);

        //  Disable the panel so viewer does not see it ingame
        if (IPanel != null)
            IPanel.gameObject.SetActive(true);
    }
    void OnDisable()
    {
        //  Remove the target from the viewer's list so viewer no longer tracks this target.
        IndicatorViewer.IndicatorTargets.Remove(this);

        //  Enable the panel so viewer can see it ingame
        if (IPanel != null)
            IPanel.gameObject.SetActive(false);
    }

    #region Initial set-up of indicator
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
            panel.name = name + " indicator";
            panel.transform.SetParent(_viewer.IndicatorCanvas.transform);
            IPanel = panel.GetComponent<IndicatorPanel>();

            //  Set the offset position of the onscreen image.
            if (IPanel.OnScreen != null)
                IPanel.OnScreen.transform.position += new Vector3(OnScreenIndicatorOffset.x, OnScreenIndicatorOffset.y, 0);

            //  Assign the initial z position. (used for scaling the indicator)
            previousZposition = IPanel.transform.position.z;

            //  Add this target to the list of targets if not already
            if (!IndicatorViewer.IndicatorTargets.Contains(this))
                IndicatorViewer.IndicatorTargets.Add(this);

            //  initial indicator toggle
            if(OnScreen(_viewer.ViewerCamera.WorldToScreenPoint(transform.position), _viewer.ViewerCamera))
            {
                if (IPanel.OnScreen != null)
                    ToggleOnScreen(true);
                if (IPanel.OffScreen != null)
                    ToggleOffScreen(false);
            }
            else
            {
                if (IPanel.OnScreen != null)
                    ToggleOnScreen(false);
                if (IPanel.OffScreen != null)
                    ToggleOffScreen(true);
            }

        }
        catch
        {   Debug.LogError("The 'DefaultIndicatorPanel' requires a UnityUI Panel object!", _viewer);    }
    }

    #endregion

    #region Update the indicator

    void Update()
    {
        //  if the viewer is currently tracking...
        if (IPanel != null && (_viewer.IsTrackingOnScreen || _viewer.IsTrackingOffScreen))
            IPanel.gameObject.SetActive(true);

        //  else if the viewer is not tracking...
        else if (IPanel != null)
            IPanel.gameObject.SetActive(false);
    }

    //  Performs the calculations to update the position & rotation of the indicator panel of this target.
    //  Called from 'IndicatorViewer' script.
    public void UpdateIndicator()
    {
        //  Get the target's world position in screen coordinate position
        Vector3 targetPosOnScreen = _viewer.ViewerCamera.WorldToScreenPoint(transform.position);

        //  if the target is visable on screen...
        if (OnScreen(targetPosOnScreen, _viewer.ViewerCamera))
        {

            //  Disable the off-screen indicator if it exist
            if (IPanel.OffScreen != null && isOffScreenEnabled)
                ToggleOffScreen(false);

            //  if the viewer allows indicators to show when the target is on-screen
            if ((forceOnScreen || _viewer.IsTrackingOnScreen) && IPanel.OnScreen != null)
            {
                //  Set target to visable.
                isVisable = true;

                //  Get distance from this target and viewer
                distanceFromViewer = GetDistance(transform.position, _viewer.ViewerObject.transform.position);

                //  If the on-screen indicator is within distance threshold, then do stuff...  
                if (CheckDisableOnDistance(distanceFromViewer) == false)
                {
                    //  Enable the on-screen indicator
                    if (!isOnScreenEnabled)
                        ToggleOnScreen(true);

                    //  set the UI panel position to the target's position
                    IPanel.transform.position = targetPosOnScreen;

                    //  If OnScreen exist && scaling is enabled && if the target's indicator axis towards the camera has changed...
                    if (_viewer.AutoScale && IPanel.transform.position.z != previousZposition)
                    {
                        UpdateScale(distanceFromViewer);

                        //  Record the new axis position of the indicator panel
                        previousZposition = IPanel.transform.position.z;
                    }
                }
                else if(isOnScreenEnabled)
                {
                    ToggleOnScreen(false);
                }
            }
            else if (IPanel.OnScreen != null && isOnScreenEnabled)
            {
                ToggleOnScreen(false);
            }
        }
        //  Else, target is Off-Screen...
        else
        {
            // Set target as invisable.
            isVisable = false;

            //  Disables the on screen indicator if it exists
            if (IPanel.OnScreen != null && isOnScreenEnabled)
                ToggleOnScreen(false);

            if ((forceOffScreen || _viewer.IsTrackingOffScreen) && IPanel.OffScreen != null)
            {
                //  Get distance from this target and viewer
                distanceFromViewer = GetDistance(transform.position, _viewer.ViewerObject.transform.position);

                //  If the off-screen indicator is within distance threshold, then do stuff...  
                if (CheckDisableOnDistance(distanceFromViewer) == false)
                {
                    //  Enables the off screen indicator
                    if (!isOffScreenEnabled)
                        ToggleOffScreen(true);

                    //  Update the indicator positon and rotation based on the target's position on the screen
                    UpdateOffScreen(targetPosOnScreen);

                    //  If scaling is enabled... then perform the update indicator method
                    if (_viewer.AutoScale)
                        UpdateScale(distanceFromViewer);
                }
                else if (isOffScreenEnabled)
                {
                    ToggleOffScreen(false);
                }
            }
            else if(IPanel.OffScreen != null && isOffScreenEnabled)
            {
                ToggleOffScreen(false);
            }     
        }
    }

    #endregion

    //  Returns true if target is within the camera screen boundaries and it's near clipping plane.
    private bool OnScreen(Vector3 pos, Camera camera)
    {
        if (pos.x < (Screen.width * _viewer.TargetEdgeDistance) && pos.x > (Screen.width - Screen.width * _viewer.TargetEdgeDistance) && 
            pos.y < (Screen.height * _viewer.TargetEdgeDistance) && pos.y > (Screen.height - Screen.height * _viewer.TargetEdgeDistance) &&
            pos.z > camera.nearClipPlane && pos.z < camera.farClipPlane)
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

    #region Update scale
    //  Updates the scaling of the UI based on distance provided
    private void UpdateScale(float distance)
    {
        //  Create a scaling factor based on distance
        float newScaleSize = _viewer.ScalingFactor / distance;

        //  Set the scale based on the scaling factor
        IPanel.transform.localScale = new Vector2(newScaleSize, newScaleSize);

        //  If the indicator scale is lower then or equal to the minimum size... Then, set the scale to the minimum size.
        //  Else if the indicator scale is greater or equal to the maximum size... Then, set the scale to the maximum size.
        if (IPanel.transform.localScale.x <= _viewer.MinScaleSize || IPanel.transform.localScale.y <= _viewer.MinScaleSize)
            IPanel.transform.localScale = new Vector2(_viewer.MinScaleSize, _viewer.MinScaleSize);
        else if (IPanel.transform.localScale.x >= _viewer.MaxScaleSize || IPanel.transform.localScale.y >= _viewer.MaxScaleSize)
            IPanel.transform.localScale = new Vector2(_viewer.MaxScaleSize, _viewer.MaxScaleSize);

        //Debug.Log("Scaling...");
    }
    #endregion

    #region Calculate OffScreen Position, Rotation, and Size

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

        //  y = mx + b (intercept forumla)
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        float m = cos / sin;

        //  Create the screen boundaries that the indicators reside in.
        Vector3 screenBounds = new Vector3(screenCenter.x * _viewer.IndicatorEdgeDistance, screenCenter.y * _viewer.IndicatorEdgeDistance);

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

        //  Assign new position
        IPanel.transform.position = new Vector3(newIndicatorPos.x, newIndicatorPos.y, targetPosOnScreen.z);

        //  Assign new rotation
        if (_viewer.RotateTowardsTargetOffScreen)
            IPanel.OffScreen.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        else
            IPanel.OffScreen.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    #endregion

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

    #region OnScreen Tranisitions

    //  Toggle On-Screen indicator
    private void ToggleOnScreen(bool enable)
    {
        //  Set its enabled state, enable/disable its gameobject, then determine which animation to use based on viewer settings
        if (enable)
        {
            isOnScreenEnabled = true;
            IPanel.OnScreen.SetActive(true);

            switch (_viewer.OnScreenEnableTransition)
            {
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OnScreen.transform, 2 * OnScreenIndicatorOffset, OnScreenIndicatorOffset, _viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OnScreen.transform, 1, _viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Rotate:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 0, 0), _viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.RotateReverse:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 0), _viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OnScreen.transform, Vector3.zero, Vector3.one, _viewer.TransitionDuration, false);
                    break;
            }
        }
        else
        {
            isOnScreenEnabled = false;

            switch (_viewer.OnScreenDisableTransition)
            {
                case IndicatorViewer.Transitions.None:
                    IPanel.OnScreen.SetActive(false);
                    break;
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OnScreen.transform, OnScreenIndicatorOffset, 2 * OnScreenIndicatorOffset, _viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OnScreen.transform, 0, _viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Rotate:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -90), _viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.RotateReverse:
                    IPanel.RotateTransition(IPanel.OnScreen.transform, Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 90), _viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OnScreen.transform, Vector3.one, Vector3.zero, _viewer.TransitionDuration, true);
                    break;
            }
        }
    }

    #endregion

    #region OffScreen Transitions

    //  //  Toggle Off-Screen indicator
    private void ToggleOffScreen(bool enable)
    {
        // Set its enabled state, enable/ disable its gameobject, then determine which animation to use based on viewer settings
        if (enable)
        {
            isOffScreenEnabled = true;
            IPanel.OffScreen.SetActive(true);
            
            switch (_viewer.OffScreenEnableTransition)
            {
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OffScreen.transform, new Vector3(0, 50, 0), Vector3.zero, _viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OffScreen.transform, 1, _viewer.TransitionDuration, false);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OffScreen.transform, Vector3.zero, Vector3.one, _viewer.TransitionDuration, false);
                    break;
            }
        }
        else
        {
            isOffScreenEnabled = false;

            switch (_viewer.OffScreenDisableTransition)
            {
                case IndicatorViewer.Transitions.None:
                    IPanel.OffScreen.SetActive(false);
                    break;
                case IndicatorViewer.Transitions.Slide:
                    IPanel.SlideTransition(IPanel.OffScreen.transform, Vector3.zero, new Vector3(0, 50, 0), _viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Fade:
                    IPanel.FadeTransition(IPanel.OffScreen.transform, 0, _viewer.TransitionDuration, true);
                    break;
                case IndicatorViewer.Transitions.Scale:
                    IPanel.ScaleTransition(IPanel.OffScreen.transform, Vector3.one, Vector3.zero, _viewer.TransitionDuration, true);
                    break;
            }
        }
    }

    #endregion

    //  Destory the indicator panel if this gameobject has been destroyed
    void OnDestroy()
    {
        DestroyIndicator();
        //print("object was destroyed");
    }

    public void DestroyIndicator()
    {
        if (IPanel != null)
            Destroy(IPanel.gameObject);
        if (GetComponent<IndicatorTargetCamera>() != null)
            Destroy(GetComponent<IndicatorTargetCamera>());
        Destroy(this);
    }

    //  Getters/Setters
    public IndicatorPanel IndicatorPanel
    {
        get { return IPanel; }
    }
    public bool IsVisable
    {
        get { return isVisable; }
    }
    public float DistanceFromViewer
    {
        get { return distanceFromViewer; }
    }
    public bool ForceOnScreen
    {
        get { return forceOnScreen; }
        set { forceOnScreen = value; }
    }
    public bool ForceOffScreen
    {
        get { return forceOffScreen; }
        set { forceOffScreen = value; }
    }
}
