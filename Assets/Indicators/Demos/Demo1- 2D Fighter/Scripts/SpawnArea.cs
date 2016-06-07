// place this on an empty GameObject
// the radius is defined by the GameObjects scale
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class SpawnArea : MonoBehaviour
{
    //  User-assigned variables
    public GameObject SpawningEntityPrefab;
    public float delayInterval = 0;
    static int maxSpawns = 10;

    public Text statusText;

    // Variables
    SphereCollider sphereCol;
    
    void Awake()
    {
        sphereCol = GetComponent<SphereCollider>();
    }

    void Start()
    {
        sphereCol.isTrigger = true;

        RestartSpawn();
    }

    IEnumerator Spawn(int maxSpawns)
    {
        int count = 0;
        while(count < maxSpawns)
        {
            GameObject Entity = Instantiate(SpawningEntityPrefab, GetRandomPosition(), Quaternion.identity) as GameObject;
            Entity.transform.SetParent(transform);
            Entity.name = "Target: " + count;
            count++;

            if (statusText != null)
                UpdateUI(count);

            yield return new WaitForSeconds(delayInterval);
        }
    }

    void UpdateUI(int currentCount)
    {
        statusText.text = "Spawned: " + currentCount + " / " + maxSpawns;
    }

    // get a random position inside the Spawn Area
    Vector3 GetRandomPosition()
    {
        Vector3 randomPos = Random.insideUnitSphere * sphereCol.radius + transform.position;
        return randomPos;
    }

    public void RestartSpawn()
    {
        if (SpawningEntityPrefab != null && maxSpawns > 0)
            StartCoroutine(Spawn(maxSpawns));
        else
            Debug.Log("No spawn prefab or MaxSpawns must be higher than 0.");
    }

    public void SetMaxSpawn(int amount)
    {
        maxSpawns = amount;
    }

    public void ResetScene()
    {
        #pragma warning disable 0618
        Application.LoadLevel(Application.loadedLevel);
        #pragma warning restore 0618
    }

    void DontDestroyOnLoad() { }
}
