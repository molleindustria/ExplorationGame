using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    DialogueManager dialogueManager;

    [Tooltip("The minimum distance to interact with an interactable")]
    public float interactionDistance = 3f;

    [Tooltip("The offset of the ray on the y axis")]
    public float rayY = 1f;


    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = GameObject.FindObjectOfType<DialogueManager>();

        if (dialogueManager == null)
            Debug.LogWarning("Warning I can't find a dialogue manager in the scene");

    }

    // Update is called once per frame
    void Update()
    {
        //raycast to see if there is a collider with an interactable attached
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        //in the editor window visualize the raycast
        Debug.DrawRay(transform.position + new Vector3(0, rayY, 0), fwd * interactionDistance, Color.green);

        RaycastHit objectHit;
        dialogueManager.currentInteractable = null;

        if (Physics.Raycast(transform.position + new Vector3(0, rayY, 0), fwd, out objectHit, interactionDistance))
        {
            //if hit something see if there is an interactable
            Interactable interactable = objectHit.collider.gameObject.GetComponent<Interactable>();

            //check if the interactable is enabled
            if (interactable != null && interactable.enabled)
            {
                //if so we hit a valid interactable
                dialogueManager.currentInteractable = interactable;
            }
        } //try a reverse raycast to detect collision from inside a collider
        else if (Physics.Raycast(transform.position + new Vector3(0, rayY, 0) + fwd * interactionDistance, -fwd, out objectHit, interactionDistance))
        {
            //if hit something see if there is an interactable
            Interactable interactable = objectHit.collider.gameObject.GetComponent<Interactable>();

            //check if the interactable is enabled
            if (interactable != null && interactable.enabled)
            {
                //if so we hit a valid interactable
                dialogueManager.currentInteractable = interactable;
            }
        }




    }
}
