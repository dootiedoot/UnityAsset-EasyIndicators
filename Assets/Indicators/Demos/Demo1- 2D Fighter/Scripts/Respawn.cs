using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Respawn : MonoBehaviour
{
    public Transform RespawnNode;

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = RespawnNode.position;
    }
}
