using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormShopManager : MonoBehaviour
{
    [Header("Shop UI")]
    public TextMeshProUGUI formNameUI;
    public Image formIcon;
    public TextMeshProUGUI priceUI;
    public TextMeshProUGUI attackStatText;
    public Slider attackStatSlider;
    public TextMeshProUGUI defenceStatText;
    public Slider defenceStatSlider;
    public TextMeshProUGUI speedStatText;
    public Slider speedStatSlider;
    public Canvas shopGUI;
    public TextMeshProUGUI balance;

    [Header("Shop buttons")]
    public Button upButton;
    public Button downButton;
    public Button buyButton;
    public Button equipButton;
    public Button unequipButton;

    [Header("Shop logic")]
    public Form[] form;
    private Form currentForm;
    public PrimaryCurrency primaryCurrency;
    public Text errorMessage;
    public float errorTimer;
    private bool timerNeedsToBeOn;
    public Canvas hud;
    public TextMeshProUGUI levelUnlocked;
    public GameObject levelUnlockedUI;
    public FormChange formChange;
    public Health playerHealth;
    public Interactor interactor;

    private void Start()
    {
        /* Stores what form the player is currently viewing so that if the player decides to equip it
         it knows what form to equip*/
        currentForm = form[0];
        UpdateGui(currentForm);
        balance.text = primaryCurrency.balance.ToString();
        errorMessage.enabled = false;
    }

    public void Update()
    {
        Timer();

        if (Input.GetKeyDown(KeyCode.K))
        {
            OpenShop();
        }
    }

    public void OpenShop()
    {
        // Current form needs to be set otherwise up and down buttons won't work
        currentForm = form[0];
        UpdateGui(currentForm);
        // Disable the HUD
        hud.gameObject.SetActive(false);
        // Enable the shop GUI
        shopGUI.gameObject.SetActive(true);
        // Display the players current balance
        balance.text = primaryCurrency.balance.ToString();
        // Enable cursor and make it visible so player can buy and equip forms in shop
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
        // When down button clicked, it starts looping through the array of different forms
        for (int i = 0; i < form.Length; i++)
        {
            // Tries to find the position of the form it is currently on
            if (currentForm.formName == form[i].formName)
            {
                // Validation in case if the end of the array has been reached
                if (i == form.Length - 1)
                {
                    // Go back to the start of the array
                    currentForm = form[0];
                }
                else
                {
                    // When it finds it, it will set the current form to the next form in the array
                    currentForm = form[i + 1];
                }

                // Updates the GUI to reflect that the player has moved on to view the next item
                UpdateGui(currentForm);
                break;
            }
        }
    }

    public void UpButton()
    {
        // When down button clicked, it starts looping through the array of different form
        for (int i = 0; i < form.Length; i++)
        {
            // Tries to find the position of the form it is currently on
            if (currentForm.formName == form[i].formName)
            {
                // Validation in case if the start of the array has been reached
                if (i == 0)
                {
                    // Go back to the end of the array
                    currentForm = form[form.Length - 1];
                }
                else
                {
                    // When it finds it, it will set the current form to the previous form in the array
                    currentForm = form[i - 1];
                }

                // Updates the GUI to reflect that the player has moved on to view the previous item
                UpdateGui(currentForm);
                break;
            }
        }
    }

    public void BuyButton()
    {
        float targetBalance = primaryCurrency.balance - currentForm.price;
        // Checks if the player has enough money to buy the form, or if they already own the form
        if (targetBalance < 0)
        {
            // If they don't have enough money, output an appropriate error message
            errorMessage.enabled = true;
            // Start a timer until the error message can disappear
            StartTimer();

            Debug.Log("Not enough money");
        }
        else if (currentForm.owned == false && currentForm.unlocked == true)
        {
            // If they have enough money, reduce their balance
            primaryCurrency.ReduceBalance(currentForm.price);

            // Loop through the array to see which item the player is buying
            for (int i = 0; i < form.Length; i++)
            { 
                if (currentForm.formName == form[i].formName)
                {
                    // Make form owned by the player so that it can't be bought again
                    form[i].owned = true;
                    currentForm.owned = true;
                }
            }
        }

        // Update the GUI so that the buy button becomes uninteractable
        UpdateGui(currentForm);
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
        // Checks if the current form has been bought by the player
        if (currentForm.owned == true)
        {
            for (int i = 0; i < form.Length; i++)
            {
                if (currentForm.formName == form[i].formName)
                {
                    // Equips the form they are viewing and updates the info of the form in the array
                    form[i].equipped = true;
                    currentForm.equipped = true;
                }
                else
                {
                    form[i].equipped = false;
                }
            }

            // Updates gui so that equip button gets disabled so they can't equip it again
            UpdateGui(currentForm);
            formChange.equippedForm = currentForm;
            playerHealth.formDefence = currentForm.defence;
        }
    }

    public void Unequip()
    {
        for (int i = 0; i < form.Length; i++)
        {
            if (currentForm.formName == form[i].formName)
            {
                // Equips the form they are viewing and updates the info of the form in the array
                form[i].equipped = false;
                currentForm.equipped = false;
            }
        }

        UpdateGui(currentForm);
        formChange.equippedForm = null;
        playerHealth.formDefence = 0;
    }

    // Updates all the shop UI referenced when it is called
    public void UpdateGui(Form form)
    {
        formNameUI.text = form.formName;
        formIcon.sprite = form.formIcon;
        priceUI.text = form.price.ToString();
        attackStatText.text = form.attack.ToString();
        attackStatSlider.value = form.attack;
        defenceStatText.text = form.defence.ToString();
        defenceStatSlider.value = form.defence;
        speedStatText.text = form.speed.ToString();
        speedStatSlider.value = form.speed;

        // If the player owns the form, disable the buy button so they can't buy it again and enable the equip button so they can buy it
        if (form.owned == true)
        {
            buyButton.interactable = false;
            equipButton.interactable = true;
        }
        // If the player does not own it, enable the buy button so they can buy it and disable the equip button so they can't equip it
        else if (form.owned == false)
        {
            buyButton.interactable = true;
            equipButton.interactable = false;
        }

        if (form.equipped == true)
        {
            // If the weapon is equipped, allow the player to unequip it and prevent them from equipping it again
            unequipButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
        }
        else if (form.equipped == false)
        {
            // If the weapon is unequipped, allow the player to equip it and prevent them from unquipping it again
            unequipButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
        }

        if (form.unlocked == true)
        {
            levelUnlockedUI.gameObject.SetActive(false);
        }
        else if (form.unlocked == false)
        {
            levelUnlockedUI.gameObject.SetActive(true);
            levelUnlocked.text = form.levelUnlocked.ToString();
        }
    }
}
