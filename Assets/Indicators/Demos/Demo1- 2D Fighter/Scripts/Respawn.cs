using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    public Transform RespawnNode;

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = RespawnNode.position;
    }
}
