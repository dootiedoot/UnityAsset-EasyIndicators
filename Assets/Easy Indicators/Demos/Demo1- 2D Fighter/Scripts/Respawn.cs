using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    public Transform RespawnNode;

    void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = RespawnNode.position;
    }
}
