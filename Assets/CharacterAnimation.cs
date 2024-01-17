using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    /*
    this script connects the character controller movement
    with the animator by setting a boolean variable
    the animator has transitions going back and forth between idle and walking 
    conditioned on that variable
    */

    public Animator animator;
    public CharacterController characterController;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //vector magnitude not zero it means it's moving in any direction
        if(characterController.velocity.magnitude > 0.1f)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }
    }
}
