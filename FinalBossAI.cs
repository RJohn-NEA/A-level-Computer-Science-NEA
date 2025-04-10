using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

public class FinalBossAI : MonoBehaviour
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
    private Vector3 initialPos;
    private Vector3 newScale = new Vector3(3, 3, 3);
    private string attackAnim;
    public Transform bossModel;

    [Header("Audio")]
    public AudioSource soundEffects;
    public AudioClip teleportSound;
    
    [Header("Form")]
    public GameObject bossAura;
    public AudioSource burstSource;
    public AudioSource auraSource;

    [Header("Meteor rain move")]
    public bool meteorRain;
    public float meteorRainCd;
    public GameObject meteorRainFX;
    public float meteorRainDamage;

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

        initialPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (belowHalfHealth && !poweredUp && enemyHealth.dead == false)
        {
            // If the boss is below half health, not dead and hasn't powered up already, start phase 2
            Phase2();
        }

        if (!poweringUp && poweredUp && enemyHealth.dead == false)
        {
            // If in phase 2
            StartTimer();
            // Start an AOE damage move every fixed number of seconds
            if (meteorRainCd <= 0)
            {
                StartCoroutine(MeteorRain());
                meteorRainCd = 15;
            }
        }
    }

    private void Update()
    {
        // Change attack animations when in phase 2
        if (!poweredUp)
        {
            attackAnim = "Attack";
            attackCd = 2;
        }
        else 
        {
            attackAnim = "Stomp";
            attackCd = 1.867f;
        }
            
        // Get distance to player
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // If the player is in range and the enemy is not dead
        if (distanceToPlayer < enemyRadius && enemyHealth.dead == false && !poweringUp && !meteorRain)
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

        // Set below half health variable to true when boss is at max of half health
        if (enemyHealth.currentHealth <= (enemyHealth.maxHealth / 2))
        {
            belowHalfHealth = true;
        }

        if (enemyHealth.currentHealth == 0)
        {
            // If the boss is dead, the player wins
            playerWins = true;
        }
        else
        {
            playerWins = false;
        }
    }

    private void StartTimer() 
    {
        meteorRainCd -= Time.deltaTime;
    }

    public void Phase2() 
    {
        // Used so that player can' attack enemy while they are powering up
        enemyHealth.invincible = true;
        poweringUp = true;
        // Display message to show they are powering up
        attackPowerUpMessage.SetActive(true);
        // Start power up animation
        animator.SetTrigger("PowerUp");
        // Activate the phase 2 effects
        bossAura.SetActive(true);
        // Play the sound effects
        burstSource.Play();
        auraSource.Play();
        // Increase the attack damage
        attackDamage += 10;
        // Increase the size of the boss slightly
        bossModel.localScale = Vector3.Lerp(bossModel.localScale, newScale, 2f * Time.deltaTime);
        // Increase the stopping distance to account for the bigger size
        enemyAgent.stoppingDistance = 2;
        StartCoroutine(StopPowerUpAnimation());
        poweredUp = true;
    }

    private void Attack(float damage)
    {
        // When attacking, the enemy should face the player
        FacePlayer();
        // If they attack cooldown isn't active
        if (readyToAttack)
        {
            // Activate the attack animation once
            animator.SetTrigger(attackAnim);
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
        // After the animation, make the boss vulnerable again
        poweringUp = false;
        attackPowerUpMessage.SetActive(false);
        // Deactivate the message.
        enemyHealth.invincible = false;
    }

    public IEnumerator MeteorRain() 
    {
        meteorRainCd = 1;
        meteorRain = true;
        // Make enemy invincible during attack
        enemyHealth.invincible = true;
        // Teleport boss to the middle of the arena
        transform.position = initialPos;
        // Playe teleport sound
        soundEffects.clip = teleportSound;
        soundEffects.Play();
        // Activate the visual effects for the move
        meteorRainFX.SetActive(true);
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyRadius);
        foreach (Collider c in colliders)
        {
            // Gets the health component of the player
            Health playerHealth = c.gameObject.GetComponentInParent<Health>();
            if (playerHealth != null) 
            {
                Debug.Log("Starting splash damage");
                StartCoroutine(splashDamage(playerHealth));
            }
        }
        Debug.Log("Doing meteor rain move");
        yield return new WaitForSeconds(5);
        // Stop the meteor move after 5 seconds
        meteorRain = false;
        enemyHealth.invincible = false;
        meteorRainFX.SetActive(false);
        soundEffects.Stop();
        Debug.Log("Stopping meteor rain move");
    }

    public IEnumerator splashDamage(Health health) 
    {
        while (meteorRain) 
        {
            // If the meteor move is active, do reoccuring splash damage to the player
            // to give the effect the meteors from the rain are doing damage to the player
            Debug.Log(health.currentHealth);
            health.TakeDamage(meteorRainDamage);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, enemyRadius);
    }
}
