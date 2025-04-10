using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeShopKeeper : MonoBehaviour, IInteractable
{
    [Header("Variables")]
    public ShopManager Manager;
    // Get melee shop manager script
    public Interactor interactor;

    public void Interact()
    {
        Manager.OpenShop();
        // Call the open shop function to activate the UI
    }
}
