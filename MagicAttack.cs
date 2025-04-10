using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicAttack : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode magicAttack = KeyCode.R;

    [Header("Ranged Attack variables")]
    public bool magicAttackPressed;
    public float magicAttackDamage;
    public float magicAttackRange;
    public Transform magicAttackSpawnPoint;
    public Transform combatLookAt;
    public float attackCooldown;
    public bool timerNeedsToOn;
    public bool readyToAttack;

    [Header("Slot Logic")]
    public MagicWeaponSlot magicWeaponSlot;
    public Slider cooldownGuiSlider;
    public Image cooldownGui;

    private void Start()
    {
        cooldownGuiSlider.maxValue = attackCooldown;
        cooldownGui.gameObject.SetActive(false);
        readyToAttack = true;
    }

    public void Update()
    {
        cooldownGuiSlider.value = 2 - attackCooldown;
        RangedAttackDetection();
        StartTimer();
        StopTimer();
    }

    public void RangedAttackDetection()
    {
        magicAttackPressed = Input.GetKeyDown(magicAttack);
        // Returns true if F key is pressed


        if (magicAttackPressed)
        {
            // Checks if the slot is empty or not to prevent a NullReferenceException error
            if (magicWeaponSlot.empty == false && readyToAttack == true)
            {
                // Get the projectile model stored in the ranged weapon slot, which is the weapon the player has equipped
                Rigidbody magicWeapon = magicWeaponSlot.rangedWeaponObj.GetComponent<Rigidbody>();
                // Create the model in the overworld at the players hand position
                Rigidbody magicWeaponInstance = Instantiate(magicWeapon, magicAttackSpawnPoint.position, magicAttackSpawnPoint.rotation) as Rigidbody;
                // Add forward force to it, force dependent on range of weapon
                magicWeaponInstance.AddForce(combatLookAt.forward * 3f, ForceMode.Impulse);
                // Get the ranged weapon behaviour component of instantiated object
                MagicWeaponBehaviour magicWeaponBehaviour = magicWeaponInstance.GetComponent<MagicWeaponBehaviour>();
                // Transfer data about position of where projectile was instantiated so it can despawn if it gets too far (prevents lag)
                magicWeaponBehaviour.instantiatePoint = magicAttackSpawnPoint.position;
                /* Transfer data from slot to the weapon itself so it can inflict damage and distance from spawn point can be checked with 
                reference to range */
                magicWeaponBehaviour.damage = magicWeaponSlot.Damage;
                magicWeaponBehaviour.range = magicWeaponSlot.range;
                StartCooldown();
            }
        }
    }

    public void StartCooldown()
    {
        timerNeedsToOn = true;
        attackCooldown = 2;
        readyToAttack = false;
    }

    public void StartTimer()
    {
        if (timerNeedsToOn)
        {
            attackCooldown -= Time.deltaTime;
            cooldownGui.gameObject.SetActive(true);
        }
    }

    public void StopTimer()
    {
        if (attackCooldown <= 0)
        {
            timerNeedsToOn = false;
            readyToAttack = true;
            cooldownGui.gameObject.SetActive(false);
        }
    }
}
