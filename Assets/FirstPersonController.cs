using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {

    [Tooltip("Move speed of the character in m/s")]
    public float movementSpeed = 5.0f;

    [Tooltip("Sprint speed")]
    public float sprintSpeed = 10.0f;

    [Tooltip("Look/rotation sensitivity via mouse")]
    public float lookSensitivity = 10f;

    [Tooltip("It can be useful when recording videos")]
    [Range(0f, 1f)]
    public float lookSmoothing = 0;

    [Tooltip("The height the player can jump")]
    public float jumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float gravity = -15.0f;

    float rotX = 0;
    float rotY = 0;
	public float upDownRange = 60.0f;
	
	float verticalVelocity = 0;

    public string jumpInput = "Jump";
    public string sprintInput = "Fire3";

    private float xVelocity = 0;
    private float yVelocity =0;
    private float MAX_SMOOTHING = 20f;
    private float TERMINAL_VELOCITY = 100f;

    CharacterController characterController;

    //freeze the controls
    public bool frozen = false;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		characterController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        if (!frozen)
        {
            rotX = Input.GetAxis("Mouse X") * lookSensitivity;
            rotY -= Input.GetAxis("Mouse Y") * lookSensitivity;

            float l = 1;
            if (lookSmoothing > 0)
                l = MAX_SMOOTHING * (1.1f - lookSmoothing) * Time.deltaTime;

            xVelocity = Mathf.Lerp(xVelocity, rotX, l);
            yVelocity = Mathf.Lerp(yVelocity, rotY, l);

            rotY = Mathf.Clamp(rotY, -upDownRange, upDownRange);
            Camera.main.transform.localRotation = Quaternion.Euler(yVelocity, 0, 0);

            transform.Rotate(0, xVelocity, 0);

            /*
            Right stick controller support. 
            1- Go to project settings > Input manager
            2- duplicate the Horizontal input by right clicking > duplicate
            3- set it to 4th axis 
            4- Rename it HorizontalLook  
            5- Do the same with VerticalLook on the 5th axis
             */

            //rotLeftRight = Input.GetAxis("HorizontalLook") * lookSensitivity/10;
            //verticalRotation = Map(Input.GetAxis("VerticalLook"), -1, 1, -upDownRange, upDownRange);

            //transform.Rotate(0, rotLeftRight, 0);

            //verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
            //Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

            float targetSpeed = movementSpeed;

            if (Input.GetButton(sprintInput))
            {
                targetSpeed = sprintSpeed;
            }

            // Movement
            float forwardSpeed = Input.GetAxis("Vertical") * targetSpeed;
            float sideSpeed = Input.GetAxis("Horizontal") * targetSpeed;

            verticalVelocity += gravity * Time.deltaTime;

            if (characterController.isGrounded && Input.GetButton(jumpInput))
            {
                //jumped
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); ;
            }
            else if (characterController.isGrounded)
            {
                //grounded
                verticalVelocity = 0;
            }
            else
            {
                //falling, clamp the speed
                if (verticalVelocity < -TERMINAL_VELOCITY)
                    verticalVelocity = -TERMINAL_VELOCITY;
            }


            Vector3 speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);

            speed = transform.rotation * speed;


            characterController.Move(speed * Time.deltaTime);

        }

    }
    
    public void FreezeControllerFor(float time)
    {
        Freeze();
        Invoke("UnfreezeController", time);
    }

    public void Freeze()
    {
        frozen = true;
    }

    public void UnFreeze()
    {
        frozen = false;
    }
    

    private float Map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

}
