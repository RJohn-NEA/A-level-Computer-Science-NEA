using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float Damage;
    public float range;
    public GameObject rangedWeaponObj;

    [Header("Shop")]
    public string weaponName;
    public Sprite weaponIcon;
    public float price;
    public int levelUnlocked; // To see if the player is the right level to be able to buy the weapon
    public Exp exp;
    public bool owned; // To check if the player has bought the item
    public bool equipped; // To check if the player is trying to equip the item
    public bool unlocked; // Used to check if the player has unlocked the weapon or not

    private void Update()
    {
        // If the players level is high enough to unlock the weapon
        if (exp.currentLevel >= levelUnlocked)
        {
            // Used in shop manager to see if they are able to buy the weapon
            unlocked = true;
        }
        else
        {
            unlocked = false;
        }
    }
}
