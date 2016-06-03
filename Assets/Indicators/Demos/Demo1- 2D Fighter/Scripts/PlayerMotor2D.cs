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

    //  References
    private CharacterController CharacterController;
    private Vector3 moveDirection = Vector3.zero;
    public bool CanDoubleJump;

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

        //  Input direction
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveDirection.y, 0);

        //  Left/Right speed
        moveDirection.x *= MoveSpeed;

        //  Jump & double jump
        if (CharacterController.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
                //Debug.Log("Jumped!");
            }

            CanDoubleJump = true;
        }
        else if(!CharacterController.isGrounded && CanDoubleJump)
        {
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
                //Debug.Log("Double Jumped!");

                CanDoubleJump = false;
            }
        }

        //  Rotation
        if (moveDirection.x < 0)
        {
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }
        else if (moveDirection.x > 0)
        {
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }

        //  Gravity
        moveDirection.y -= gravity * Time.deltaTime;

        CharacterController.Move(moveDirection * Time.deltaTime);
    }
}
