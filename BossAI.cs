using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("Enemy AI")]
    public NavMeshAgent enemyAgent;
    public float enemyRadius;
    public GameObject player;
    private float distanceToPlayer;
    public Animator animator;
    private Health playerHealth;
    [HideInInspector] public EnemyHealth enemyHealth;
    private bool belowHalfHealth;
    private bool poweredUp;
    private bool poweringUp;
    public bool playerWins;
    public GameObject forceField;

    [Header("UI")]
    public GameObject attackPowerUpMessage;

    [Header("Attack variables")]
    public float attackDamage;
    public float attackCd;
    public bool readyToAttack;

    private void Start()
    {
        // Get health from player at the start
        playerHealth = player.GetComponent<Health>();

        // Get enemy's health script at the start
        enemyHealth = GetComponent<EnemyHealth>();

        readyToAttack = true;
    }

    private void Update()
    {
        // Get distance to player
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // If the player is in range and the enemy is not dead
        if (distanceToPlayer < enemyRadius && enemyHealth.dead == false && !poweringUp && playerHealth.dead == false)
        {
            // Enemy should go to player
            enemyAgent.SetDestination(player.transform.position);
            // Activate walking animation
            animator.SetBool("IsWalking", true);

            // If the enemy has reached the player
            if (distanceToPlayer <= enemyAgent.stoppingDistance)
            {
                // Enemy should stop
                enemyAgent.SetDestination(transform.position);
                // Deactivate the walking animation
                animator.SetBool("IsWalking", false);
                // Attack the player
                Attack(attackDamage);
            }
        }
        else
        {
            // Stop walking
            animator.SetBool("IsWalking", false);
            // Stay still
            enemyAgent.SetDestination(transform.position);
        }

        if (enemyHealth.currentHealth <= (enemyHealth.maxHealth/2)) 
        {
            belowHalfHealth = true;
        }

        if (belowHalfHealth && !poweredUp && enemyHealth.dead == false)
        {
            enemyHealth.invincible = true;
            poweringUp = true;
            attackPowerUpMessage.SetActive(true);
            animator.SetTrigger("PowerUp");
            attackDamage += 5;
            StartCoroutine(StopPowerUpAnimation());
            poweredUp = true;
        }

        if (enemyHealth.currentHealth == 0)
        {
            playerWins = true;
        }
        else 
        {
            playerWins = false;
        }
    }

    private void Attack(float damage)
    {
        // When attacking, the enemy should face the player
        FacePlayer();
        // If they attack cooldown isn't active
        if (readyToAttack)
        {
            // Activate the attack animation once
            animator.SetTrigger("Attack");
            // Subtract health from the players current health, done with a pre-existing function made earlier
            playerHealth.TakeDamage(damage);
            Debug.Log(playerHealth.currentHealth);
            // Make sure enemy can't immediately attack again
            readyToAttack = false;
            // Call the reset attack function
            StartCoroutine(ResetAttack());
        }
    }

    public IEnumerator ResetAttack()
    {
        // Wait the anount of time the attack cooldown is
        yield return new WaitForSeconds(attackCd);
        // Make it so the enemy can attack again
        readyToAttack = true;
    }

    private void FacePlayer()
    {
        // Get the direction the player is from the enemy
        Vector3 direction = (player.transform.position - transform.position).normalized;
        // Calculate the rotation the enemy should be to look in the direction of the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Set the enemy rotation to the look rotation, using slerp function so it has smoothing and not immediate and snappy
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public IEnumerator StopPowerUpAnimation() 
    {
        yield return new WaitForSeconds(2.367f);
        poweringUp = false;
        attackPowerUpMessage.SetActive(false);
        enemyHealth.invincible = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, enemyRadius);
    }
}
