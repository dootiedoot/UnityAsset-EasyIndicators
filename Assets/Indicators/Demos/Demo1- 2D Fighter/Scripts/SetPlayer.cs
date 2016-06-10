using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetPlayer : MonoBehaviour
{
    public string Name;

    // Use this for initialization
    void Start()
    {
        //  Set the text of the player's indicator
        if (GetComponent<IndicatorTarget>() != null)
            StartCoroutine(CoSetPlayerName());
    }

    IEnumerator CoSetPlayerName()
    {
        //  find the panel until it is not null
        IndicatorPanel IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;

        while (IPanel == null)
        {
            IPanel = GetComponent<IndicatorTarget>().IndicatorPanel;
            yield return null;
        }

        //  Change name
        IPanel.OnScreen.GetComponentInChildren<Text>(true).text = Name;
    }
}
