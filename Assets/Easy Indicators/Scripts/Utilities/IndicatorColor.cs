using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//  This script is used to assign a random color and it's indicator UI.
//  Simply attach this to any target that has the 'IndicatorTarget' component on it.

public class IndicatorColor : MonoBehaviour
{
    //  Settings & options
    [Header("Settings")]
    [Tooltip("The color of the indicator.")]
    public Color Color = Color.red;
    [Tooltip("Use a random color. Will override 'NewColor'")]
    public bool RandomColor = false;
    [Tooltip("Should this gameobject be set to the new color?")]
    public bool ChangeGameobjectColor = false;
    [Tooltip("Should this gameobject's children be set to the new color?")]
    public bool ChangeChildrenColor = false;

    // Use this for initialization
	void Start ()
    {
        ChangeColor(Color, RandomColor, ChangeGameobjectColor, ChangeChildrenColor);
    }

    public void ChangeColor(Color newColor, bool random, bool changeGO, bool changeChildren)
    {
        //  Get a new random color if enabled
        if (random)
            newColor = new Color(UnityEngine.Random.Range(0f, 1f), Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

        //  Change color of this gameobject.
        if (changeGO)
        {
            //  Gameobject
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().material.color = newColor;
        }

        //  Change color of this gameobject's children
        if (changeChildren)
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renders.Length; i++)
                renders[i].material.color = newColor;
        }

        //  Change the indicator color if it exsist
        if (GetComponent<IndicatorTarget>() != null)
            StartCoroutine(CoChangeColor(newColor));
    }

    #region IEnumerator that checks for a indicator panel that may not have been created yet thus we need to keep checking till it exist.
    IEnumerator CoChangeColor(Color newColor)
    {
        //yield return new WaitForSeconds(0.15f);

        //  Change color of all the indicator panel items
        IndicatorPanel IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;

        while (IPanel == null)
        {
            IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
            yield return null;
        }

        //  Changes all graphic colors
        Graphic[] graphics = IPanel.GetComponentsInChildren<Graphic>(true);
        if (graphics.Length > 0)
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].color = newColor;
    }
    #endregion
}
