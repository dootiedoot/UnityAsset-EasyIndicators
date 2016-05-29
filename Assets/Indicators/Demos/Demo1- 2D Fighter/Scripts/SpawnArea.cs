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
    public float delayInterval;
    public int maxSpawns;

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

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
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
}
