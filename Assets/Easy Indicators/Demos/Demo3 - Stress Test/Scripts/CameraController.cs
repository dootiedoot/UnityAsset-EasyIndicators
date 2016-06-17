using UnityEngine;
using System.Collections;

// This script requires a character controller to be attached
[RequireComponent (typeof (CharacterController))]
public class CameraController : MonoBehaviour
{
    public float speed = 6.0F;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), GetZoomInput());

        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    int GetZoomInput()
    {
        if (Input.GetKey(KeyCode.Q))
            return -1;
        if (Input.GetKey(KeyCode.E))
            return 1;
        return 0;
    }
}

