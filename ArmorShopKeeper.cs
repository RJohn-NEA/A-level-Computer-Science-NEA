using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorShopKeeper : MonoBehaviour, IInteractable
{
    [Header("Variables")]
    public ArmorShopManager Manager;
    // Get armor shop manager script

    public void Interact() 
    {
        Manager.OpenShop();
        // Call the open shop function to activate the UI
    }
}
