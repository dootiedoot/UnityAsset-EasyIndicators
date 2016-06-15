﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(IndicatorTarget))]
public class IndicatorDistanceTracker : MonoBehaviour
{
    //  User-assigned variables
    [Header("User-Assigned Variables")]
    [Tooltip("The custom font used for the distance indicator. If left empty, the default Arial font will be used instead.")]
    public Font CustomFont;

    //  Settings & options
    [Header("Settings")]
    [Tooltip("Does the distance show on the On-Screen indicator?")]
    public bool ShowOnScreen = true;
    [Tooltip("Does the distance show on the Off-Screen indicator?")]
    public bool ShowOffScreen = true;
    [Tooltip("Offset position for the on-screen indicator.")]
    public Vector2 OnScreenPosOffset = new Vector2(0, 65);
    [Tooltip("Offset position for the off-screen indicator.")]
    public Vector2 OffScreenPosOffset = new Vector2(0, -50);
    [Tooltip("The font size of distancetext.")]
    public int FontSize = 14;
    [Tooltip("The suffix text that displays after the distance text. Represents distance units such as Meters.")]
    public string distanceSuffix = "m";
    [Tooltip("The farthest decimal point the distance will display.")]
    public Decial DecimalPoint;
    public enum Decial
    { None, Tenths, Hundredths, Thousandths, TenThousandths }

    //  Variables
    private IndicatorTarget ITarget;
    private IndicatorPanel IPanel;
    private GameObject distanceIndicator;
    private Text distanceText;
    private bool isOnScreen = true;

	void Awake()
    {
        //  Find & Assign references.
        ITarget = GetComponent<IndicatorTarget>();    
    }
    
    // Use this for initialization
	void Start ()
    {
        StartCoroutine(CoCreateDistanceTracker());
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if ((ShowOnScreen || ShowOffScreen) && IPanel != null)
        {
            //  Runs once during on-screen & off-screen transition
            if (ITarget.IsVisable && IPanel.OnScreen != null && !isOnScreen)
            {
                if (ShowOnScreen)
                {
                    distanceIndicator.SetActive(true);    
                    distanceIndicator.transform.SetParent(IPanel.OnScreen.transform);
                    distanceIndicator.transform.localPosition = OnScreenPosOffset;
                }
                else
                    distanceIndicator.SetActive(false);

                isOnScreen = true;
            }
            else if (!ITarget.IsVisable && IPanel.OffScreen != null && isOnScreen)
            {
                if (ShowOffScreen)
                {
                    distanceIndicator.SetActive(true);
                    distanceIndicator.transform.SetParent(IPanel.OffScreen.transform);
                    distanceIndicator.transform.localPosition = OffScreenPosOffset;
                }
                else
                    distanceIndicator.SetActive(false);

                isOnScreen = false;
            }

            //  Set the decimal point
            switch (DecimalPoint)
            {
                case Decial.None:
                    distanceText.text = ITarget.DistanceFromViewer.ToString("F0") + distanceSuffix;
                    break;
                case Decial.Tenths:
                    distanceText.text = ITarget.DistanceFromViewer.ToString("F1") + distanceSuffix;
                    break;
                case Decial.Hundredths:
                    distanceText.text = ITarget.DistanceFromViewer.ToString("F2") + distanceSuffix;
                    break;
                case Decial.Thousandths:
                    distanceText.text = ITarget.DistanceFromViewer.ToString("F3") + distanceSuffix;
                    break;
                case Decial.TenThousandths:
                    distanceText.text = ITarget.DistanceFromViewer.ToString("F4") + distanceSuffix;
                    break;
            }

            distanceIndicator.transform.localScale = Vector3.one;
            distanceIndicator.transform.rotation = Quaternion.identity;
        }    
	}

    //  Using ienumerator because the indicator panel may not have been created yet thus we need to keep checking till it exist.
    IEnumerator CoCreateDistanceTracker()
    {
        IPanel = ITarget.IndicatorPanel;

        while (IPanel == null)
        {
            IPanel = ITarget.IndicatorPanel;
            yield return null;
        }

        //  Now that the indicator panel exist, create the tracker
        CreateDistanceTracker();
    }

    void CreateDistanceTracker()
    {
        distanceIndicator = new GameObject("DistanceTrackerIndicator");
        distanceIndicator.layer = 1 << 4;
        distanceIndicator.transform.SetParent(ITarget.IndicatorPanel.transform);
        distanceIndicator.transform.localPosition = Vector3.zero;
        distanceIndicator.transform.localScale = Vector3.one;

        //  text
        distanceText = distanceIndicator.AddComponent<Text>();
        distanceText.alignment = TextAnchor.MiddleCenter;
        distanceText.horizontalOverflow = HorizontalWrapMode.Overflow;
        distanceText.verticalOverflow = VerticalWrapMode.Overflow;
        distanceText.fontSize = FontSize;
        if (CustomFont == null)
        {
            CustomFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            distanceText.font = CustomFont;
        }

        distanceIndicator.SetActive(false);
    }
}