using UnityEngine;
using System.Collections;

// This script requires a character controller to be attached
[RequireComponent (typeof (CharacterController))]
public class PlayerMotorTD : MonoBehaviour
{
    public Transform rotatedModel;
    public float speed = 6.0F;
    public float gravity = 20.0F;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //  Rotation
        if(moveDirection != Vector3.zero && rotatedModel != null)
            rotatedModel.transform.rotation = Quaternion.LookRotation(moveDirection);

        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}

