using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// An interface is created that other scripts can use and be derived from
interface IInteractable 
{
    public void Interact();
    // Any scripts that derive from this interface must have an interact function
}

public class Interactor : MonoBehaviour
{
    [Header("Raycast variables")]
    private RaycastHit hitInfo;
    public Transform orientation;

    [Header("Interaction")]
    public float interactionRange;
    public KeyCode interactionKey = KeyCode.E;
    // Key used to interact with interactable objects
    public LayerMask interactionLayer;
    public bool IsInteracting;

    [Header("UI")]
    public GameObject interactionUI;

    public void FixedUpdate()
    {
        // If an interactable object is in range
        if (Physics.Raycast(transform.position, orientation.forward, out hitInfo, interactionRange, interactionLayer))
        {
            // Turn on the interaction UI so the player knows they can interact with it and press E to do so
            interactionUI.SetActive(true);

            // If they do press E
            if (Input.GetKeyDown(interactionKey) && IsInteracting == false)
            {
                IsInteracting = true;
                // Try get the interaction interface from the script attached to the object, if it derives from the interface
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    // Call the interact function that they are required to have if they derive from the interactable interface
                    interactObj.Interact();
                }
            }
        }
        else
        {
            // If interactable object is not in range, disable interaction UI
            interactionUI.SetActive(false);
        }
    }
}
