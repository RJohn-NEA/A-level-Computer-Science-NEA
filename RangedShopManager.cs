using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RangedShopManager : MonoBehaviour
{
    [Header("Shop UI")]
    public TextMeshProUGUI weaponNameUI;
    public Image weaponIcon;
    public TextMeshProUGUI priceUI;
    public TextMeshProUGUI attackStatText;
    public Slider attackStatSlider;
    public TextMeshProUGUI rangeStatText;
    public Slider rangeStatSlider;
    public Canvas shopGUI;
    public TextMeshProUGUI balance;

    [Header("Shop buttons")]
    public Button upButton;
    public Button downButton;
    public Button buyButton;
    public Button equipButton;
    public Button unequipButton;

    [Header("Shop logic")]
    public RangedWeapon[] rangedWeapon;
    private RangedWeapon currentWeapon;
    public PrimaryCurrency primaryCurrency;
    public Text errorMessage;
    public float errorTimer;
    private bool timerNeedsToBeOn;
    public RangedWeaponSlot rangedWeaponSlot;
    public Canvas hud;
    public TextMeshProUGUI levelUnlocked;
    public GameObject levelUnlockedUI;
    public Interactor interactor;

    private void Start()
    {
        /* Stores what weapon the player is currently viewing so that if the player decides to equip it
         it knows what weapon to equip*/
        currentWeapon = rangedWeapon[0];
        UpdateGui(currentWeapon);
        balance.text = primaryCurrency.balance.ToString();
        errorMessage.enabled = false;
    }

    public void Update()
    {
        Timer();

        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        // Current weapon needs to be set otherwise up and down buttons won't work
        currentWeapon = rangedWeapon[0];
        UpdateGui(currentWeapon);
        // Disable the HUD
        hud.gameObject.SetActive(false);
        // Enable the shop GUI
        shopGUI.gameObject.SetActive(true);
        // Display the players current balance
        balance.text = primaryCurrency.balance.ToString();
        // Enable cursor and make it visible so player can buy and equip weapons in shop
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
        // When down button clicked, it starts looping through the array of different melee weapons
        for (int i = 0; i < rangedWeapon.Length; i++)
        {
            // Tries to find the position of the weapon it is currently on
            if (currentWeapon.weaponName == rangedWeapon[i].weaponName)
            {
                // Validation in case if the end of the array has been reached
                if (i == rangedWeapon.Length - 1)
                {
                    // Go back to the start of the array
                    currentWeapon = rangedWeapon[0];
                }
                else
                {
                    // When it finds it, it will set the current weapon to the next weapon in the array
                    currentWeapon = rangedWeapon[i + 1];
                }

                // Updates the GUI to reflect that the player has moved on to view the next item
                UpdateGui(currentWeapon);
                break;
            }
        }
    }

    public void UpButton()
    {
        // When down button clicked, it starts looping through the array of different melee weapons
        for (int i = 0; i < rangedWeapon.Length; i++)
        {
            // Tries to find the position of the weapon it is currently on
            if (currentWeapon.weaponName == rangedWeapon[i].weaponName)
            {
                // Validation in case if the start of the array has been reached
                if (i == 0)
                {
                    // Go back to the end of the array
                    currentWeapon = rangedWeapon[rangedWeapon.Length - 1];
                }
                else
                {
                    // When it finds it, it will set the current weapon to the previous weapon in the array
                    currentWeapon = rangedWeapon[i - 1];
                }

                // Updates the GUI to reflect that the player has moved on to view the previous item
                UpdateGui(currentWeapon);
                break;
            }
        }
    }

    public void BuyButton()
    {
        float targetBalance = primaryCurrency.balance - currentWeapon.price;
        // Checks if the player has enough money to buy the weapon, or if they already own the weapon
        if (targetBalance < 0)
        {
            // If they don't have enough money, output an appropriate error message
            errorMessage.enabled = true;
            // Start a timer until the error message can disappear
            StartTimer();

            Debug.Log("Not enough money");
        }
        else if (currentWeapon.owned == false && currentWeapon.unlocked == true)
        {
            // If they have enough money, reduce their balance
            primaryCurrency.ReduceBalance(currentWeapon.price);

            // Loop through the array to see which item the player is buying
            for (int i = 0; i < rangedWeapon.Length; i++)
            {
                if (currentWeapon.weaponName == rangedWeapon[i].weaponName)
                {
                    // Make weapon owned by the player so that it can't be bought again
                    rangedWeapon[i].owned = true;
                    currentWeapon.owned = true;
                }
            }
        }

        // Update the GUI so that the buy button becomes uninteractable
        UpdateGui(currentWeapon);
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
        // Checks if the current weapon has been bought by the player
        if (currentWeapon.owned == true)
        {
            for (int i = 0; i < rangedWeapon.Length; i++)
            {
                if (currentWeapon.weaponName == rangedWeapon[i].weaponName)
                {
                    // Equips the weapon they are viewing and updates the info of the weapon in the array
                    rangedWeapon[i].equipped = true;
                    currentWeapon.equipped = true;
                    // Sends data about weapon to the weapon item slot
                    rangedWeaponSlot.AddItem(rangedWeapon[i]);
                }
                else
                {
                    rangedWeapon[i].equipped = false;
                }
            }

            // Updates gui so that equip button gets disabled so they can't equip it again
            UpdateGui(currentWeapon);
        }
    }

    public void Unequip()
    {
        for (int i = 0; i < rangedWeapon.Length; i++)
        {
            if (currentWeapon.weaponName == rangedWeapon[i].weaponName)
            {
                // Equips the weapon they are viewing and updates the info of the weapon in the array
                rangedWeapon[i].equipped = false;
                currentWeapon.equipped = false;
                // Sends data about weapon to the weapon item slot
                rangedWeaponSlot.RemoveItem();
            }
        }

        UpdateGui(currentWeapon);

    }

    // Updates all the shop UI referenced when it is called
    public void UpdateGui(RangedWeapon rW)
    {
        weaponNameUI.text = rW.weaponName;
        weaponIcon.sprite = rW.weaponIcon;
        priceUI.text = rW.price.ToString();
        attackStatText.text = rW.Damage.ToString();
        attackStatSlider.value = rW.Damage;
        rangeStatText.text = rW.range.ToString();
        rangeStatSlider.value = rW.range;

        // If the player owns the weapon, disable the buy button so they can't buy it again and enable the equip button so they can buy it
        if (rW.owned == true)
        {
            buyButton.interactable = false;
            equipButton.interactable = true;
        }
        // If the player does not own it, enable the buy button so they can buy it and disable the equip button so they can't equip it
        else if (rW.owned == false)
        {
            buyButton.interactable = true;
            equipButton.interactable = false;
        }

        if (rW.equipped == true)
        {
            // If the weapon is equipped, allow the player to unequip it and prevent them from equipping it again
            unequipButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
        }
        else if (rW.equipped == false)
        {
            // If the weapon is unequipped, allow the player to equip it and prevent them from unquipping it again
            unequipButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
        }

        if (rW.unlocked == true)
        {
            levelUnlockedUI.gameObject.SetActive(false);
        }
        else if (rW.unlocked == false)
        {
            levelUnlockedUI.gameObject.SetActive(true);
            levelUnlocked.text = rW.levelUnlocked.ToString();
        }

        // To change size of image so proportion is correct
        //(Hardcoded solution)
        if (rW.weaponName == "Kunai")
        {
           // Set size so it is correct proportion
            RectTransform rT = weaponIcon.GetComponent<RectTransform>();
            rT.sizeDelta = new Vector2(205, 95);
        }
        else
        {
            // Set size so it is correct proportion
            RectTransform rT = weaponIcon.GetComponent<RectTransform>();
            rT.sizeDelta = new Vector2(100, 95);
        }
    }
}
