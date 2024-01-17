using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Full speed")]
    public float movementSpeed = 10;

    [Tooltip("Prevent diagonal movements")]
    public bool fourDirection = false;

    [Tooltip("Make it into a side scroller")]
    public bool twoDirection = false;

    [Tooltip("Set true to reach full speed instantly")]
    public bool analogSpeed = true;

    [Tooltip("Rotation speed: 0 for no rotation 500+ for instant")]
    public float rotationSpeed = 30;

    [Tooltip("Sprint speed")]
    public float sprintSpeed = 10.0f;

    [Tooltip("The height the player can jump: 0 for no jump")]
    public float jumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float gravity = -15.0f;

    //a complex component that facilitates the control of a character
    public CharacterController controller;

    public Vector2 movementInput;

    public float verticalVelocity = 0;

    [Tooltip("If set to true prevents any movements")]
    public bool frozen = false;

    private float initialZ = 0;

    // Start is called before the first frame update
    void Start()
    {
        //add a reference to the controller component at the beginning
        controller = GetComponent<CharacterController>();

        initialZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (!frozen)
        {
            float targetSpeed = movementSpeed;

            //change speed based on sprint input 
            if (Input.GetButtonDown("Fire3"))
            {
                targetSpeed = sprintSpeed;
            }
            
            //push it down constantly or the isGrounded may not work
            verticalVelocity += gravity * Time.deltaTime;


            //jump if active - if two direction you can also use "up" for jump
            if (jumpHeight > 0 && controller.isGrounded && (Input.GetButtonDown("Fire2") || (twoDirection && Input.GetAxisRaw("Vertical")>0)))
            {
                //calculate the jump velocity based on the desired jump height
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            //create a 2D vector with the movement input (analog stick, arrows, or WASD) 
            movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            //if not analog speed overrides unity's axis smoothing (emulating analog stick) by reading the raw input
            if (!analogSpeed)
            {
                //both movement components can only be 0 or 1
                movementInput = Vector2.zero;

                if (Input.GetAxisRaw("Horizontal") > 0)
                    movementInput.x = 1;

                if (Input.GetAxisRaw("Horizontal") < 0)
                    movementInput.x = -1;

                if (Input.GetAxisRaw("Vertical") > 0)
                    movementInput.y = 1;

                if (Input.GetAxisRaw("Vertical") < 0)
                    movementInput.y = -1;
            }

            //to limit movement to 4 directions simply zero the smaller component
            if (fourDirection)
            {
                if (Mathf.Abs(movementInput.x) >= Mathf.Abs(movementInput.y))
                    movementInput = new Vector2(movementInput.x, 0);
                else
                    movementInput = new Vector2(0, movementInput.y);

            }

            //lower the camer to make a side scrolling view
            if (twoDirection)
            {
                movementInput = new Vector2(movementInput.x, 0);
            }

            //combining the left stick input and the vertical velocity
            //absolute coordinates movement: up means +z in the world, left means -x
            Vector3 movement = new Vector3(movementInput.x * targetSpeed, 0, movementInput.y * targetSpeed);

            //limit the speed to avoid diagonal movements being slightly faster
            movement = Vector3.ClampMagnitude(movement, targetSpeed);

            //add gravity or jump
            movement = movement + new Vector3(0, verticalVelocity, 0);

            //since it's in update and continuous the vector has to be multiplied by Time.deltaTime to be frame independent
            controller.Move(movement * Time.deltaTime);

            //prevent colliders from pushing the player away from the inizial z position
            if(twoDirection)
            {
                controller.enabled = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, initialZ);
                controller.enabled = true;

            }

            //if any input at all
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0)
            {
                //direction based on controller velocity
                //Vector3 lookDirection = new Vector3(controller.velocity.x, 0, controller.velocity.z);

                //direction based on input, better for targeting interactables
                Vector3 lookDirection = new Vector3(movement.x, 0, movement.z);

                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }

            }
        }
    }


    //these functions can be called externally to block the controls
    public void Freeze()
    {
        frozen = true;
        //stop
        controller.SimpleMove(Vector3.zero);
    }

    public void UnFreeze()
    {
        frozen = false;
    }
}
