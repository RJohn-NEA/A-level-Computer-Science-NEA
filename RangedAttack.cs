using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RangedAttack : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode rangedAttack = KeyCode.F;

    [Header("Ranged Attack variables")]
    public bool rangedAttackPressed;
    public float rangedAttackDamage;
    public float rangedAttackRange;
    public Transform rangedAttackSpawnPoint;
    public Transform combatLookAt;
    public float attackCooldown;
    public bool timerNeedsToOn;
    public bool readyToAttack;

    [Header("Slot Logic")]
    public RangedWeaponSlot rangedWeaponSlot;
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
        rangedAttackPressed = Input.GetKeyDown(rangedAttack);
        // Returns true if F key is pressed


        if (rangedAttackPressed) 
        {
            // Checks if the slot is empty or not to prevent a NullReferenceException error
            if (rangedWeaponSlot.empty == false && readyToAttack == true) 
            {
                // Get the projectile model stored in the ranged weapon slot, which is the weapon the player has equipped
                Rigidbody rangedWeapon = rangedWeaponSlot.rangedWeaponObj.GetComponent<Rigidbody>();
                // Create the model in the overworld at the players hand position
                Rigidbody rangedWeaponInstance = Instantiate(rangedWeapon, rangedAttackSpawnPoint.position, rangedAttackSpawnPoint.rotation) as Rigidbody;
                // Add forward force to it, force dependent on range of weapon
                rangedWeaponInstance.AddForce(combatLookAt.forward * 9f, ForceMode.Impulse);
                // Add a rotational force to the object to make it spin so it looks like it is being thrown
                rangedWeaponInstance.AddTorque(0f, -10f, 0f, ForceMode.Impulse);
                // Get the ranged weapon behaviour component of instantiated object
                RangedWeaponBehaviour rangedWeaponBehaviour = rangedWeaponInstance.GetComponent<RangedWeaponBehaviour>();
                // Transfer data about position of where projectile was instantiated so it can despawn if it gets too far (prevents lag)
                rangedWeaponBehaviour.instantiatePoint = rangedAttackSpawnPoint.position;
                /* Transfer data from slot to the weapon itself so it can inflict damage and distance from spawn point can be checked with 
                reference to range */
                rangedWeaponBehaviour.damage = rangedWeaponSlot.Damage;
                rangedWeaponBehaviour.range = rangedWeaponSlot.range;
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
