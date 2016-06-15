using UnityEngine;
using System.Collections;

public class Demo4Buttons : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void StartTracking()
    {
        IndicatorViewer.StartTracking();
    }
    public void StopTracking()
    {
        IndicatorViewer.StopTracking();
    }
}
