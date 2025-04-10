using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode meleeAttack = KeyCode.Mouse0;

    [Header("Attack Variables")]
    public bool meleeAttackPressed;
    public float baseMeleeAttackDamage;
    public float baseMeleeAttackRange;
    private float meleeAttackDamage;
    private float meleeAttackRange;
    public float meleeAttackDamageMultiplier = 1.1f;
    public float formAttack;
    
    [Header("Enemy Detection")]
    private bool enemyInRange;
    private RaycastHit Enemyhit;
    public Transform combatLookAt;
    public LayerMask enemyLayer;

    [Header("Combo mechanic")]
    public float timeBetweenAttacks;
    public int attackCounter;
    public bool readyToAttack;
    public float comboCooldown;
    private bool timerNeedsToBeOn;
    private bool comboCooldownNeedsToBeOn;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI acText;

    [Header("Attack cooldown")]
    public float attackCooldown;
    public bool attackTimerNeedsToBeOn;

    [Header("Animation")]
    public Animator animator;
    private string attack1;
    private string attack2;
    private string attack3;

    [Header("Slot logic")]
    public MeleeWeaponSlot weaponSlot;

    public Rigidbody rb;
    public FormChange formChange;
    public bool conversationActive;

    private void Start()
    {
        readyToAttack = true;
        comboText.enabled = false;
        acText.enabled = false;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        SetAttackStats();
        AttackDetection();
        AttackCooldown();

    }

    public void attack(GameObject gameObject, float attackDamage)
    {
        // Access the health component of the enemy that has been collided with
        EnemyHealth enemyHealth = gameObject.GetComponent<EnemyHealth>();
        // Calculates the theoretical new health the enemy should be at after the attack
        float targetHealth = enemyHealth.currentHealth - attackDamage;

        if (targetHealth < 0)
        {
            // Validation to see if health will go into negatives after attack
            enemyHealth.currentHealth = 0;
            // If it does, just set health to 0
        }
        else
        {
            // Otherwise apply the damage to the object and deduct health points from it
            enemyHealth.currentHealth = targetHealth;
        }

        // Testing purposes
        Debug.Log(gameObject.name + " is at " + enemyHealth.currentHealth + " health");
    }

    public void StartTimer() 
    {
        timeBetweenAttacks = 2;
        timerNeedsToBeOn = true;
    }

    public void StartComboCooldown() 
    {
        comboCooldown = 5;
        comboCooldownNeedsToBeOn = true;
    }
    
    public void ResetCombo()
    {
        readyToAttack = true;
        attackCounter = 0;
        comboCooldownNeedsToBeOn = false;
        comboCooldown = 5;
        comboText.enabled = false;
        acText.enabled = false;
    }

    public void AttackDetection() 
    {
        meleeAttackPressed = Input.GetKeyDown(meleeAttack);
        // Returns true if the left mouse button is clicked
        Vector3 combatLookAtDir = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
        // Calculates which way the player is facing
        enemyInRange = Physics.SphereCast(transform.position, 0.5f, combatLookAtDir.normalized, out Enemyhit, meleeAttackRange, enemyLayer);
        // Returns true if an enemy is in the specified range

        if (meleeAttackPressed && !attackTimerNeedsToBeOn && !conversationActive)
        {
            if (readyToAttack) 
            {
                if (attackCounter == 0)
                {
                    animator.SetTrigger(attack1);
                }
                else if (attackCounter == 1)
                {
                    animator.SetTrigger(attack2);
                }
                else if (attackCounter == 2)
                {
                    animator.SetTrigger(attack3);
                }
            }

            if (enemyInRange)
            {
                GameObject enemy = Enemyhit.transform.gameObject;
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemy != null && enemyHealth.dead == false && enemyHealth.invincible == false)
                {
                    if (readyToAttack)
                    {
                        if (timeBetweenAttacks < 2 && timeBetweenAttacks > 0)
                        {
                            attack(enemy, (meleeAttackDamage + formAttack) * Mathf.Pow(meleeAttackDamageMultiplier, attackCounter));
                        }
                        else
                        {
                            attack(enemy, meleeAttackDamage + formAttack);
                        }

                        if (!formChange.formCharged && formChange.equippedForm != null && !formChange.formChanged)
                        {
                            formChange.formProgress += 0.125f;
                        }

                        StartTimer();
                        // Starts a timer

                        if (attackCounter != 3)
                        {
                            attackCounter += 1;
                        }

                        // Keeps track of the number of times the player has attacked to see if they reach a combo

                        Debug.Log(attackCounter);
                        acText.text = attackCounter.ToString();

                        if (attackCounter > 0)
                        {
                            acText.enabled = true;
                            if (attackCounter == 3)
                            {
                                comboText.enabled = true;
                                StartComboCooldown();
                                readyToAttack = false;
                            }
                        }
                    }
                }
            }

            if (attackCounter != 3) 
            {
                StartAttackCooldown();
            }
        }

        if (comboCooldownNeedsToBeOn) 
        {
            comboCooldown -= Time.deltaTime;
        }
        
        if (comboCooldown < 0)
        {
            ResetCombo();
        }
        
        if (timerNeedsToBeOn)
        {
            timeBetweenAttacks -= Time.deltaTime;
        }

        if (timeBetweenAttacks < 0)
        {
            attackCounter = 0;
            timeBetweenAttacks = 2;
            timerNeedsToBeOn = false;

            if (!comboCooldownNeedsToBeOn) 
            {
                comboText.enabled = false;
                acText.enabled = false;
            }
        }
    }

    public void SetAttackStats() 
    {
        // Checks if the melee weapon slot it empty or not
        if (weaponSlot.empty == false)
        {
            // If it is not, use the stats stored in the weapon slot which are from the weapon equipped from the shop
            meleeAttackDamage = weaponSlot.Damage;
            meleeAttackRange = weaponSlot.range;
            ChangeAttackAnimations("Sword");
        }
        else if (weaponSlot.empty == true) 
        {
            // If it is, use the base attack damage and range
            meleeAttackDamage = baseMeleeAttackDamage;
            meleeAttackRange = baseMeleeAttackRange;
            ChangeAttackAnimations("Attack");
        }
    }

    public void StartAttackCooldown() 
    {
        attackCooldown = 1.2f;
        attackTimerNeedsToBeOn = true;
    }

    public void AttackCooldown() 
    {
        if (attackTimerNeedsToBeOn) 
        { 
            attackCooldown -= Time.deltaTime;
        }

        if (attackCooldown < 0) 
        {
            attackTimerNeedsToBeOn = false;
            attackCooldown = 1.2f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void ChangeAttackAnimations(string animation) 
    {
        attack1 = animation + "1";
        attack2 = animation + "2";
        attack3 = animation + "3";
    }
}
