using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedShopKeeper : MonoBehaviour, IInteractable
{
    [Header("Variables")]
    public RangedShopManager Manager;
    // Get ranged shop manager script

    public void Interact()
    {
        Manager.OpenShop();
        // Call the open shop function to activate the UI
    }
}
