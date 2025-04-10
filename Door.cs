using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
    // Derives from interactable interface
{
    [Header("Variables")]
    public GameObject[] objectsToDeactivate;
    // An array so multiple objects can be deactivated if needed
    public GameObject[] objectsToActivate;
    // An array so multiple objects can be activated if needed
    public Transform doorSpawnPoint;
    // An object in the overworld that has the position to teleport the player to
    public Transform player;
    // Stores the players transform component
    public Interactor interactor;

    // Called from interactor script on player if they are in range and press E

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void Interact()
    {
        interactor.IsInteracting = false;

        foreach (GameObject objectToActivate in objectsToActivate)
        {
            objectToActivate.SetActive(true);
            // Activate the objects in the array
        }

        player.position = doorSpawnPoint.position;
        // Set the players position based on what the door is to

        foreach (GameObject objectToDeactivate in objectsToDeactivate)
        {
            objectToDeactivate.SetActive(false);
            /* Deactivate the objects in the array after the player has teleported
               so they don't fall off*/
        }
    }
}
