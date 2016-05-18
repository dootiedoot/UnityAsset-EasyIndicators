using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TargetIndicator : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-assigned variables")]
    [Tooltip("The custom indicator UI Panel shown only on this target which overrides the global default. If left empty, the global default will be used instead.")]
    public GameObject CustomIndicatorPanel;
    [Tooltip("Offset position of the indicator UI from the target.")]
    public Vector3 IndicatorOffset;

    //  Variables
    private GameObject IndicatorPanel;
    private GameObject IndicatorImage;
    private bool isVisable;
    
    //  References
    private ViewerIndicator _viewerIndicator;

    void Awake()
    {
        _viewerIndicator = FindObjectOfType<ViewerIndicator>();
    }

    void Start()
    {
        InitializeIndicator();
        //foreach (TargetIndicator targets in _viewerIndicator.Targets)
        //    Debug.Log(targets.name);
    }

    //  OnEnable/OnDisable used for pooling
    void OnEnable()
    {
        if (!_viewerIndicator.Targets.Contains(this))
            _viewerIndicator.Targets.Add(this);
        if (IndicatorPanel != null)
            IndicatorPanel.SetActive(true);
    }

    void OnDisable()
    {
        _viewerIndicator.Targets.Remove(this);
        IndicatorPanel.SetActive(false);
    }

    private void InitializeIndicator()
    {

        //  If no custom indicator image is assigned to this gameobject, use the default one from the 'ViewerIndicator' script.
        if (CustomIndicatorPanel == null)
            CustomIndicatorPanel = _viewerIndicator.DefaultIndicatorPanel;

        //  If customIndicatorPanel is still null because there is no global default, throw error 
        try
        {
            //  Create the runtime indicator object and assign it under the canvas. 
            IndicatorPanel = Instantiate(CustomIndicatorPanel, Vector2.zero, Quaternion.identity) as GameObject;
            IndicatorPanel.transform.SetParent(_viewerIndicator.DefaultIndicatorCanvas.transform);
            IndicatorImage = IndicatorPanel.GetComponent<IndicatorPanel>().IndicatorImage;

            //  EXTRA FUN STUFF FOR DEBUGGING
            IndicatorImage.GetComponent<Image>().color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }
        catch
        {
            Debug.LogError("The 'DefaultIndicatorPanel' requires a UnityUI Panel Gameobject!", _viewerIndicator);
        }
    }

    //  Performs the calculations to update the position & rotation of the indicator of this gameobject.
    //  Called from 'ViewerIndicator' script.
    public void UpdateIndicator(Camera ViewerCamera)
    {
        Vector3 targetPosOnScreen = ViewerCamera.WorldToScreenPoint(transform.position);

        //  if the target is visable on screen...
        if (OnScreen(targetPosOnScreen, ViewerCamera.nearClipPlane))
        {
            //  if the viewer allows indicators to show when the target is visable...
            if (_viewerIndicator.ShowOnVisable)
            {
                //  Enable the indicator image and set it as invisable.
                IndicatorPanel.SetActive(true);
                isVisable = true;
                //Debug.Log("Target is visable.");
                //float distance = 250 / GetDistance(transform.position, _viewerIndicator.transform.position);
                //Debug.Log(GetDistance(transform.position, _viewerIndicator.transform.position));
                IndicatorPanel.transform.position = targetPosOnScreen + IndicatorOffset;
                IndicatorImage.transform.rotation = Quaternion.Euler(0, 0, 180);
                //Indicator.transform.localScale = new Vector2(distance, distance);
            }
            else
            {
                //  Disable the indicator image and set it as invisable.
                IndicatorPanel.SetActive(false);
                isVisable = false;
            }
        }
        else
        {
            //  Enable the indicator image and set it as invisable.
            IndicatorPanel.SetActive(true);
            isVisable = false;

            //  Create a variable for the center position of the screen.
            Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

            //  Set targetPosOnScreen anchor to center instead of bottom left
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
            Vector3 screenBounds = screenCenter * _viewerIndicator.DistanceFromEdge;

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
            IndicatorPanel.transform.position = targetPosOnScreen;
            IndicatorImage.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
    }

    //  Checks and returns true if target is within the camera screen boundaries.
    private bool OnScreen(Vector3 pos, float nearClipPlane)
    {
        if (pos.x < Screen.width && pos.x > 0 && 
            pos.y < Screen.height && pos.y > 0 &&
            pos.z > nearClipPlane)
            return true;
        return false;
    }

    //  Returns the distance between two vector3 positions. (Fast & optimized)
    private float GetDistance(Vector3 PosA, Vector3 PosB)
    {
        Vector3 heading;

        heading.x = PosA.x - PosB.x;
        heading.y = PosA.y - PosB.y;
        heading.z = PosA.z - PosB.z;

        float distanceSquared = heading.x * heading.x + heading.y * heading.y + heading.z * heading.z;
        return Mathf.Sqrt(distanceSquared);
    }

    //  Getters/Setters
    public GameObject GetIndicatorPanel()
    { return IndicatorPanel; }

    public bool IsVisable()
    { return isVisable; }
}
