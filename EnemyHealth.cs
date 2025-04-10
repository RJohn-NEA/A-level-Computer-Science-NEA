using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Variables")]
    public float currentHealth;
    public float maxHealth;
    public PrimaryCurrency currency;
    public int rewardCurrency;
    public Exp exp;
    public float rewardExp;
    private Vector3 intialPos;
    public GameObject healthbar;
    public Image healthBarFill;
    public Animator animator;
    public bool dead;
    public bool invincible;
    public bool boss;
    public QuestSlot playerQuestSlot;
    public bool addedToKillCount;
    public string enemyType;

    private void Start()
    {
        // Set initial position
        intialPos = transform.position;
        // Set enemy to max health
        currentHealth = maxHealth;

        addedToKillCount = false;
    }

    private void Update()
    {
        // Update healthbar UI
        healthBarFill.fillAmount = currentHealth / maxHealth;
        
        if (currentHealth == 0) 
        {
            // Die
            Die();
        }

        // Only set healthbar active if enemy has taken any damage
        if (!boss)
        {
            if (currentHealth < maxHealth)
            {
                healthbar.SetActive(true);
            }
            else
            {
                healthbar.SetActive(false);
            }
        }
        else 
        {
            healthbar.SetActive(true);

            if (currentHealth == 0) 
            {
                healthbar.SetActive(false);
            }
        }
    }

    public void Die() 
    {
        // Deactivate object after death animation played
        StartCoroutine(PlayDeathAnimation());
        
        // If a quest is active to kill this enemy, update the kill counter
        if (playerQuestSlot.currentQuest != null && playerQuestSlot.currentQuest.enemy.enemyType == enemyType)
        {
            if (!addedToKillCount) 
            {
                Debug.Log("Added to kill count");

                playerQuestSlot.UpdateKillCounter();
                addedToKillCount = true;
            }
        }
    }

    public IEnumerator PlayDeathAnimation() 
    {
        dead = true;
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(3.7f);
        dead = false;
        // Set the position of the enemy to it's start position
        transform.position = intialPos;

        if (!boss) 
        {
            // Reset enemy's health so it is full for when it is reactivated and respawned
            currentHealth = maxHealth;
        }

        // Reward player with money
        currency.AddToBalance(rewardCurrency);
        // Reward player with experience points
        exp.AddEXP(rewardExp);
        gameObject.SetActive(false);
    }

}
