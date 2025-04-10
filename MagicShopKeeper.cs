using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShopKeeper : MonoBehaviour, IInteractable
{
    [Header("Variables")]
    public MagicShopManager Manager;
    // Get magic shop manager script

    public void Interact()
    {
        Manager.OpenShop();
        // Call the open shop function to activate the UI
    }
}
