using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormShopKeeper : MonoBehaviour, IInteractable
{
    [Header("Variables")]
    public FormShopManager Manager;
    // Get form shop manager script

    public void Interact()
    {
        Manager.OpenShop();
        // Call the open shop function to activate the UI
    }
}
