using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class ArmorShopManager : MonoBehaviour
{
    [Header("Shop UI")]
    public TextMeshProUGUI armorNameUI;
    public Image armorIcon;
    public TextMeshProUGUI priceUI;
    public TextMeshProUGUI defenceStatText;
    public Slider defenceStatSlider;
    public Canvas shopGUI;
    public TextMeshProUGUI balance;

    [Header("Shop buttons")]
    public Button upButton;
    public Button downButton;
    public Button buyButton;
    public Button equipButton;
    public Button unequipButton;

    [Header("Shop logic")]
    public Armor[] armor;
    private Armor currentArmor;
    public PrimaryCurrency primaryCurrency;
    public Text errorMessage;
    public float errorTimer;
    private bool timerNeedsToBeOn;
    public Canvas hud;
    public TextMeshProUGUI levelUnlocked;
    public GameObject levelUnlockedUI;
    public Health playerHealth;
    public Interactor interactor;

    private void Start()
    {
        /* Stores what armor the player is currently viewing so that if the player decides to equip it
         it knows what armor to equip*/
        currentArmor = armor[0];
        UpdateGui(currentArmor);
        balance.text = primaryCurrency.balance.ToString();
        errorMessage.enabled = false;
    }

    public void Update()
    {
        CheckForArmor();
        Timer();
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            OpenShop();
        }
    }

    public void CheckForArmor() 
    {
        bool armorEquipped = false;

        for (int i = 0; i < armor.Length; i++)
        {
            if (i != 0)
            {
                if (armor[i].equipped == true)
                {
                    // Set defence in health to the defence stat of armor
                    playerHealth.armorDefence = armor[i].defence;
                    armorEquipped = true;
                    break;
                }
                else
                {
                    armorEquipped = false;
                }
            }
        }

        if (armorEquipped == false)
        {
            // If no armor equipped, equip starter armor automatically so player isn't invisible
            armor[0].equipped = true;
            playerHealth.armorDefence = armor[0].defence;
        }
    }

    public void OpenShop()
    {
        // Current armor needs to be set otherwise up and down buttons won't work
        currentArmor = armor[0];
        UpdateGui(currentArmor);
        // Disable the HUD
        hud.gameObject.SetActive(false);
        // Enable the shop GUI
        shopGUI.gameObject.SetActive(true);
        // Display the players current balance
        balance.text = primaryCurrency.balance.ToString();
        // Enable cursor and make it visible so player can buy and equip armors in shop
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        // Disable the shop GUI
        shopGUI.gameObject.SetActive(false);
        // Enable the HUD GUI again
        hud.gameObject.SetActive(true);
        // Get rid of cursor so player can move camera properly
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        interactor.IsInteracting = false;
    }

    public void DownButton()
    {
        // When down button clicked, it starts looping through the array of different armors
        for (int i = 0; i < armor.Length; i++)
        {
            // Tries to find the position of the armor it is currently on
            if (currentArmor.armorName == armor[i].armorName)
            {
                // Validation in case if the end of the array has been reached
                if (i == armor.Length - 1)
                {
                    // Go back to the start of the array
                    currentArmor = armor[0];
                }
                else
                {
                    // When it finds it, it will set the current armor to the next armor in the array
                    currentArmor = armor[i + 1];
                }

                // Updates the GUI to reflect that the player has moved on to view the next item
                UpdateGui(currentArmor);
                break;
            }
        }
    }

    public void UpButton()
    {
        // When down button clicked, it starts looping through the array of different armors
        for (int i = 0; i < armor.Length; i++)
        {
            // Tries to find the position of the armor it is currently on
            if (currentArmor.armorName == armor[i].armorName)
            {
                // Validation in case if the start of the array has been reached
                if (i == 0)
                {
                    // Go back to the end of the array
                    currentArmor = armor[armor.Length - 1];
                }
                else
                {
                    // When it finds it, it will set the current armor to the previous armor in the array
                    currentArmor = armor[i - 1];
                }

                // Updates the GUI to reflect that the player has moved on to view the previous item
                UpdateGui(currentArmor);
                break;
            }
        }
    }

    public void BuyButton()
    {
        float targetBalance = primaryCurrency.balance - currentArmor.price;
        // Checks if the player has enough money to buy the armor, or if they already own the armor
        if (targetBalance < 0)
        {
            // If they don't have enough money, output an appropriate error message
            errorMessage.enabled = true;
            // Start a timer until the error message can disappear
            StartTimer();

            Debug.Log("Not enough money");
        }
        else if (currentArmor.owned == false && currentArmor.unlocked == true)
        {
            // If they have enough money, reduce their balance
            primaryCurrency.ReduceBalance(currentArmor.price);

            // Loop through the array to see which item the player is buying
            for (int i = 0; i < armor.Length; i++)
            {
                if (currentArmor.armorName == armor[i].armorName)
                {
                    // Make armor owned by the player so that it can't be bought again
                    armor[i].owned = true;
                    currentArmor.owned = true;
                }
            }
        }

        // Update the GUI so that the buy button becomes uninteractable
        UpdateGui(currentArmor);
        balance.text = primaryCurrency.balance.ToString();
    }

    public void StartTimer()
    {
        errorTimer = 2f;
        timerNeedsToBeOn = true;
    }

    public void Timer()
    {
        if (timerNeedsToBeOn == true)
        {
            // Starts timer
            errorTimer -= Time.deltaTime;
        }

        if (errorTimer < 0)
        {
            // Turns timer off
            timerNeedsToBeOn = false;
            // Disables error message
            errorMessage.enabled = false;
        }
    }

    public void Equip()
    {
        // Checks if the current armor has been bought by the player
        if (currentArmor.owned == true)
        {
            for (int i = 0; i < armor.Length; i++)
            {
                if (currentArmor.armorName == armor[i].armorName)
                {
                    // Equips the armor they are viewing and updates the info of the armor in the array
                    armor[i].equipped = true;
                    currentArmor.equipped = true;
                }
                else
                {
                    armor[i].equipped = false;
                }
            }

            // Updates gui so that equip button gets disabled so they can't equip it again
            UpdateGui(currentArmor);
        }
    }

    public void Unequip()
    {
        for (int i = 0; i < armor.Length; i++)
        {
            if (currentArmor.armorName == armor[i].armorName)
            {
                // Equips the armor they are viewing and updates the info of the armor in the array
                armor[i].equipped = false;
                currentArmor.equipped = false;
            }
        }

        UpdateGui(currentArmor);

    }

    // Updates all the shop UI referenced when it is called
    public void UpdateGui(Armor aW)
    {
        armorNameUI.text = aW.armorName;
        armorIcon.sprite = aW.armorIcon;
        priceUI.text = aW.price.ToString();
        defenceStatText.text = aW.defence.ToString();
        defenceStatSlider.value = aW.defence;

        // If the player owns the armor, disable the buy button so they can't buy it again and enable the equip button so they can buy it
        if (aW.owned == true)
        {
            buyButton.interactable = false;
            equipButton.interactable = true;
        }
        // If the player does not own it, enable the buy button so they can buy it and disable the equip button so they can't equip it
        else if (aW.owned == false)
        {
            buyButton.interactable = true;
            equipButton.interactable = false;
        }

        if (aW.equipped == true)
        {
            // If the armor is equipped, allow the player to unequip it and prevent them from equipping it again
            unequipButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
        }
        else if (aW.equipped == false)
        {
            // If the armor is unequipped, allow the player to equip it and prevent them from unquipping it again
            unequipButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
        }

        if (aW.unlocked == true)
        {
            levelUnlockedUI.gameObject.SetActive(false);
        }
        else if (aW.unlocked == false)
        {
            levelUnlockedUI.gameObject.SetActive(true);
            levelUnlocked.text = aW.levelUnlocked.ToString();
        }

        if (aW.armorName == "Starter Armor") 
        {
            unequipButton.interactable = false;
        }
    }
}
