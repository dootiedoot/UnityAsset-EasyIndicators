using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DemoSwitcher : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeDemo(int demoID)
    {
        switch (demoID)
        {
            case 0:
                SceneManager.LoadScene(0);
                break;
            case 1:
                SceneManager.LoadScene(1);
                break;
            case 2:
                SceneManager.LoadScene(2);
                break;
            case 3:
                SceneManager.LoadScene(3);
                break;
            default:
                break;
        }
    }
}
