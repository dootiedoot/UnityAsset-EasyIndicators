﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//  This script is used to assign a random color and it's indicator UI.
//  Simply attach this to any target that has the 'IndicatorTarget' component on it.

public class IndicatorColorAssigner : MonoBehaviour
{
    //  Settings & options
    [Header("Settings")]
    [Tooltip("The color of the indicator.")]
    public Color NewColor = Color.red;
    [Tooltip("Use a random color. Will override 'NewColor'")]
    public bool RandomColor = false;
    [Tooltip("Should this gameobject be set to the new color?")]
    public bool ChangeGameobjectColor = false;
    [Tooltip("Should this gameobject's children be set to the new color?")]
    public bool ChangeChildrenColor = false;


    // Use this for initialization
	void Start ()
    {
        //  Get a new random color if enabled
        if (RandomColor)
            NewColor = new Color(UnityEngine.Random.Range(0f, 1f), Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

        //  Change color of this gameobject.
        if (ChangeGameobjectColor)
        {
            //  Gameobject
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().material.color = NewColor;
        }

        //  Change color of this gameobject's children
        if (ChangeChildrenColor)
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renders.Length; i++)
                renders[i].material.color = NewColor;    
        }

        //  Change the indicator color if it exsist
        if (GetComponent<IndicatorTarget>() != null)
            StartCoroutine(ChangeColor(NewColor));
    }

    //  Using ienumerator because the indicator panel may not have been created yet thus we need to keep checking. Will stop when color is finally changed.
    IEnumerator ChangeColor(Color newColor)
    {
        //  Change color of all the indicator panel items
        IndicatorPanel IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;

        while (IPanel == null)
        {
            IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
            yield return null;
        }
        
        //  Changes all image colors
        Image[] images = IPanel.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
            images[i].color = newColor;

        //  Change all text colors
        Text[] texts = IPanel.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
            texts[i].color = newColor;
    }
}