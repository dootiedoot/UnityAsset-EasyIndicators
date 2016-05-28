using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.5f;

    private float accum = 0.0f;
    private int frames = 0; 
    private float timeleft;

    private Text Text;

    void Awake()
    {
        Text = GetComponent<Text>();
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0)
        {
            // display two fractional digits (f2 format)
            Text.text = "FPS - " + (accum / frames).ToString("f2");
            timeleft = updateInterval;
            accum = 0;
            frames = 0;
        }
    }
}
