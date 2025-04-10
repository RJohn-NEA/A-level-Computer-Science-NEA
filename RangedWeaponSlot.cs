using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RangedWeaponSlot : MonoBehaviour
{
    [Header("Slot GUI")]
    public Image weaponIcon;
    public TextMeshProUGUI attackStatText;
    public TextMeshProUGUI attackText;

    [Header("Ranged Weapon Data")]
    public float Damage;
    public float range;
    [HideInInspector] public GameObject rangedWeaponObj;

    [Header("Slot logic")]
    public bool empty;

    public void Start()
    {
        // Checks if the slot is empty or not at the start
        if (weaponIcon.IsActive() == false)
        {
            empty = true;
        }
    }

    public void AddItem(RangedWeapon rW)
    {
        // Store weapon stats in slot to be used in attack script
        Damage = rW.Damage;
        range = rW.range;
        rangedWeaponObj = rW.rangedWeaponObj;
        // Show what weapon the player has equipped when not in shop
        weaponIcon.sprite = rW.weaponIcon;
        weaponIcon.gameObject.SetActive(true);
        // Show stats of weapon player has equipped when not in shop
        attackStatText.text = Damage.ToString();
        attackText.gameObject.SetActive(true);
        attackStatText.gameObject.SetActive(true);
        // Needed for attack script to see if slot is empty
        empty = false;
    }

    public void RemoveItem()
    {
        // Hide GUI to show player has nothing equipped
        weaponIcon.gameObject.SetActive(false);
        attackText.gameObject.SetActive(false);
        attackStatText.gameObject.SetActive(false);
        // Slot is empty
        empty = true;
    }
}
