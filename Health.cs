using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Healthbar")]
    public Slider slider;
    public float currentHealth;
    public float maxHealth;

    public Rigidbody rb;
    // Set from armor shop manager
    public float defence;
    public float armorDefence;
    public float formDefence;
    public bool dead;
    public TextMeshProUGUI deathMessage;
    public Vector3 respawnPoint;
    private float smallestDistance;
    public GameObject playerModel;

    public bool playerGotAttacked;
    public float healthTimer;
    public float healthRegenTime;
    public bool healthTimerNeedsToBeOn;

    public RespawnPoint[] respawnPoints;
    public float[] distanceFromRespawnPoints;

    [Header("References")]
    public PlayerMovement pm;
    public Attack playerAttack;
    public Dashing dashing;
    public RangedAttack ra;
    public MagicAttack ma;
    public FormChange formChange;

    public void Start()
    {
        // Set the healthbar
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
    }

    public void Update()
    {
        // Update the healthbar GUI so it reflects changes to hp
        slider.value = currentHealth;

        CheckIfPlayerGotAttacked();
        HealthTimer();
    }

    private void FixedUpdate()
    {
        if (currentHealth == 0)
        {
            dead = true;
            StartCoroutine(Death());
        }

        if (Input.GetKeyDown(KeyCode.B)) 
        {
            Debug.Log(Vector3.Distance(transform.position, respawnPoints[0].respawnPosition));
        }
    }

    public void TakeDamage(float damage)
    {

        defence = (armorDefence + formDefence) * 0.01f;
        defence = 1 - defence;
        
        float targetHealth;
        // Take into account the defence of the players armor
        targetHealth = currentHealth - (damage * defence);
        
        // Validation so health doesn't go below 0
        if (currentHealth != 0)
        {
            if (targetHealth >= 0)
            {
                currentHealth = targetHealth;
            }
            else if (targetHealth < 0)
            {
                // If the enemy does more damage than the players health, set its health to 0 so it doesn't go below 0
                currentHealth = 0;
            }
        }

        playerGotAttacked = true;
    }

    public void HealthTimer() 
    {
        if (healthTimerNeedsToBeOn)
            healthTimer -= Time.deltaTime;
    }
    
    public void CheckIfPlayerGotAttacked() 
    {
        if (playerGotAttacked) 
        {
            healthTimer = 30f;
            healthTimerNeedsToBeOn = true;
            playerGotAttacked = false;
        }

        if (healthTimer <= 0 && healthTimerNeedsToBeOn) 
        {
            StartCoroutine(RegenHealth());
            healthTimerNeedsToBeOn = false;
        }
    }

    public IEnumerator RegenHealth() 
    {
        while (currentHealth != maxHealth) 
        {
            float targetHealth = currentHealth + 5;
            if (targetHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else 
            {
                currentHealth = targetHealth;
            }

            yield return new WaitForSeconds(healthRegenTime);
        }
    }

    public IEnumerator Death()
    {
        currentHealth = maxHealth;
        deathMessage.gameObject.SetActive(true);
        formChange.DeactivateIfDead();
        pm.enabled = false;
        playerAttack.enabled = false;
        dashing.enabled = false;
        ra.enabled = false;
        ma.enabled = false;
        playerModel.SetActive(false);
        yield return new WaitForSeconds(5);
        deathMessage.gameObject.SetActive(false);
        dead = false;
        Respawn();
    }

    public void Respawn() 
    {
        for (int i = 0; i < respawnPoints.Length; i++)
        {
            if (respawnPoints[i].activated)
            {
                distanceFromRespawnPoints[i] = Vector3.Distance(transform.position, respawnPoints[i].respawnPosition);
            }
            else
            {
                distanceFromRespawnPoints[i] = -1f;
            }
        }

        smallestDistance = distanceFromRespawnPoints[0];
        respawnPoint = respawnPoints[0].respawnPosition;

        for (int i = 0; i < distanceFromRespawnPoints.Length; i++)
        {
            if (distanceFromRespawnPoints[i] != -1)
            {
                if (smallestDistance > distanceFromRespawnPoints[i])
                {
                    smallestDistance = distanceFromRespawnPoints[i];
                    respawnPoint = respawnPoints[i].respawnPosition;
                }
            }
        }

        transform.position = respawnPoint;
        playerModel.SetActive(true);
        pm.enabled = true;
        playerAttack.enabled = true;
        dashing.enabled = true;
        ra.enabled = true;
        ma.enabled = true;
    }
}
