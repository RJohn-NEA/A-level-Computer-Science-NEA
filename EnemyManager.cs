using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float respawnTime;
    private bool battleEnded;
    public TextMeshProUGUI win_loseMessage;
    public Health playerHealth;

    [Header("Enemies Set 1")]
    public EnemyHealth[] set1;
    private bool[] set1Deaths;

    [Header("Enemies Set 2")]
    public EnemyHealth[] set2;
    private bool[] set2Deaths;

    [Header("Enemies Set 3")]
    public EnemyHealth[] set3;
    private bool[] set3Deaths;

    [Header("Enemies Set 4")]
    public EnemyHealth[] set4;
    private bool[] set4Deaths;

    [Header("Enemies Set 5")]
    public EnemyHealth[] set5;
    private bool[] set5Deaths;

    [Header("Enemies Set 6")]
    public EnemyHealth[] set6;
    private bool[] set6Deaths;

    [Header("Bosses")]
    public BossAI[] bossesAI;

    [Header("Final Boss")]
    public FinalBossAI finalBoss;

    private void Start()
    {
        set1Deaths = new bool[set1.Length];
        set2Deaths = new bool[set2.Length];
        set3Deaths = new bool[set3.Length];
        set4Deaths = new bool[set4.Length];
        set5Deaths = new bool[set5.Length];
        set6Deaths = new bool[set6.Length];
    }

    private void Update()
    {
        EnemyTracker();
    }

    public void EnemyTracker()
    {
        Set1Tracker();
        Set2Tracker();
        Set3Tracker();
        Set4Tracker();
        Set5Tracker();
        Set6Tracker();
        BossTracker();
    }

    public void Set1Tracker()
    {
        for (int i = 0; i < set1.Length; i++)
        {
            if (set1[i].gameObject.activeSelf == false)
            {
                set1Deaths[i] = true;
            }
            else
            {
                set1Deaths[i] = false;
            }

            if (set1Deaths[i] == true)
            {
                StartCoroutine(Respawn(set1[i]));
            }
        }
    }

    public void Set2Tracker()
    {
        for (int i = 0; i < set2.Length; i++)
        {
            if (set2[i].gameObject.activeSelf == false)
            {
                set2Deaths[i] = true;
            }
            else
            {
                set2Deaths[i] = false;
            }

            if (set2Deaths[i] == true)
            {
                StartCoroutine(Respawn(set2[i]));
            }
        }
    }

    public void Set3Tracker()
    {
        for (int i = 0; i < set3.Length; i++)
        {
            if (set3[i].gameObject.activeSelf == false)
            {
                set3Deaths[i] = true;
            }
            else
            {
                set3Deaths[i] = false;
            }

            if (set3Deaths[i] == true)
            {
                StartCoroutine(Respawn(set3[i]));
            }
        }
    }

    public void Set4Tracker()
    {
        for (int i = 0; i < set4.Length; i++)
        {
            if (set4[i].gameObject.activeSelf == false)
            {
                set4Deaths[i] = true;
            }
            else
            {
                set4Deaths[i] = false;
            }

            if (set4Deaths[i] == true)
            {
                StartCoroutine(Respawn(set4[i]));
            }
        }
    }

    public void Set5Tracker()
    {
        for (int i = 0; i < set5.Length; i++)
        {
            if (set5[i].gameObject.activeSelf == false)
            {
                set5Deaths[i] = true;
            }
            else
            {
                set5Deaths[i] = false;
            }

            if (set5Deaths[i] == true)
            {
                StartCoroutine(Respawn(set5[i]));
            }
        }
    }

    public void Set6Tracker()
    {
        for (int i = 0; i < set6.Length; i++)
        {
            if (set6[i].gameObject.activeSelf == false)
            {
                set6Deaths[i] = true;
            }
            else
            {
                set6Deaths[i] = false;
            }

            if (set6Deaths[i] == true)
            {
                StartCoroutine(Respawn(set6[i]));
            }
        }
    }

    public void BossTracker()
    {
        for (int i = 0; i < bossesAI.Length; i++)
        {
            if (bossesAI[i].playerWins == true)
            {
                battleEnded = true;
            }

            if (battleEnded)
            {
                StartCoroutine(EndBattle(bossesAI[i]));
                EnemyHealth enemyHealth = bossesAI[i].enemyHealth;
                enemyHealth.currentHealth = enemyHealth.maxHealth;
                battleEnded = false;
            }
        }
    }

    public void FinalBossTracker() 
    {
        if (finalBoss.playerWins) 
        {
            battleEnded = true;
        }
    }

    public IEnumerator Respawn(EnemyHealth enemy) 
    {
        BasicEnemy enemyAI = enemy.GetComponent<BasicEnemy>();
        enemyAI.readyToAttack = true;
        enemy.addedToKillCount = false;
        yield return new WaitForSeconds(respawnTime);
        enemy.gameObject.SetActive(true);
    }

    public IEnumerator EndBattle(BossAI bossAI) 
    {
        win_loseMessage.gameObject.SetActive(true);
        win_loseMessage.text = "Victory";
        yield return new WaitForSeconds(3.7f);
        bossAI.forceField.SetActive(false);
        win_loseMessage.gameObject.SetActive(false);
    }
}
