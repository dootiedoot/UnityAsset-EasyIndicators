using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Demo4TargetSwitcher : MonoBehaviour
{

    public GameObject[] targets;
    Dropdown dropList;

    void Awake()
    {
        dropList = GetComponent<Dropdown>();
    }

    void Start()
    {
        changeTarget();
    }

    public void changeTarget()
    {
        switch (dropList.value)
        {
            case 0:
                Demo4Settings.currentTarget = targets[0];
                break;
            case 1:
                Demo4Settings.currentTarget = targets[1];
                break;
            case 2:
                Demo4Settings.currentTarget = targets[2];
                break;
            default:
                break;
        }

        Debug.Log("Current target: " + Demo4Settings.currentTarget);
    }
}
