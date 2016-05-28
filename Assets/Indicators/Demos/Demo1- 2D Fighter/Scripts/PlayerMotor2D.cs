using UnityEngine;
using System.Collections;

// This script requires a character controller to be attached
[RequireComponent(typeof(CharacterController))]
public class PlayerMotor2D : MonoBehaviour
{
    // Attributes
    public float MoveSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;

    //  References
    private CharacterController CharacterController;

    void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, 90, 0);
    }

    void FixedUpdate()
    {
        if (CharacterController.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
    
            if (moveDirection.x < 0)
            {
                transform.localRotation = Quaternion.Euler(0, -90, 0);
            }
            else if (moveDirection.x > 0)
            {
                transform.localRotation = Quaternion.Euler(0, 90, 0);
            }

            moveDirection *= MoveSpeed;

            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        CharacterController.Move(moveDirection * Time.deltaTime);
    }
}
