using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    [Header("Quest")]
    public Quest currentQuest;
    public bool questActive;
    public bool questComplete;
    public int currentKillCount;
    public LayerMask playerLayer;

    [Header("Quest UI")]
    public GameObject questUI;
    public TextMeshProUGUI questNameUI;
    public TextMeshProUGUI questDescriptionUI;
    public TextMeshProUGUI questCompleteUI;
    public GameObject questProgressUI;
    public TextMeshProUGUI killCountUI;

    [Header("Quest givers")]
    public QuestGiver[] questGivers;

    [Header("Waypoint arrow")]
    public GameObject waypointArrow;
    public bool destinationReached;
    public float playerDetectionRadius;

    private void Start()
    {
        currentKillCount = 0;
    }

    private void Update()
    {
        if (currentQuest != null)
        {
            questUI.SetActive(true);
            // Whenever there is a quest in the slot, activate the quest UI
            questNameUI.text = currentQuest.questName;
            // Set the name text to the name of the quest
            questDescriptionUI.text = currentQuest.questDescription;
            // Set the description text to the description of the quest

            if (currentQuest.type == Quest.questType.kill)
            {
                // If it is a kill quest, we want a progress update UI so the player knows how many kills they are on
                questProgressUI.gameObject.SetActive(true);
                // Updated by using the current kill count variable
                killCountUI.text = currentKillCount.ToString() + " / " + currentQuest.targetKillCount;
                // Checks is the player has killed the required amount of enemies set in the quest
                CheckIfKillQuestDone();
            }
            else if (currentQuest.type == Quest.questType.travel)
            {
                // Don't need a kill count UI if it is a travel quest
                questProgressUI.SetActive(false);
                // Check if the player has reached their destination
                CheckIfTravelQuestDone();
            }
        }
        else 
        {
            // If the quest slot is empty, deactivate the quest UI
            questUI.SetActive(false);
        }

        StopQuest();
    }

    private void FixedUpdate()
    {
        if (currentQuest != null)
        {
            // If the quest slot is not empty, activate a guidance system to show where the player needs to go
            ActivateWayPointArrow();
        }
        else 
        {
            // If it is empty, activate the guidance system
            waypointArrow.SetActive(false);
        }
    }

    public void ActivateWayPointArrow() 
    {
        // Activate arrow so player knows where to go
        waypointArrow.SetActive(true);

        // Get direction of target location for quest from the player
        Vector3 direction = (currentQuest.targetLocation - transform.position).normalized;
        // Calculate the rotation the arrow should be to point to the target location
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Change the rotation of the arrow smoothly using the slerp() function
        waypointArrow.transform.rotation = Quaternion.Slerp(waypointArrow.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void UpdateKillCounter() 
    {
        // Called from the respective enemy health script
        currentKillCount++;
    }

    public void CheckIfKillQuestDone() 
    {
        if (currentKillCount == currentQuest.targetKillCount) 
        {
            // If the player has killed the required amount of enemy's, the quest has been completed
            questComplete = true;
        }
    }

    public void CheckIfTravelQuestDone() 
    {
        destinationReached = Physics.CheckSphere(currentQuest.targetLocation, playerDetectionRadius, playerLayer);

        if (destinationReached) 
        {
            // If the player's transform is in a suitable range to the target location, the quest has been completed
            questComplete = true;
        }
    }

    public void StopQuest() 
    {
        if (questComplete)
        {
            // Reset the kill count
            currentKillCount = 0;
            // Set quest complete to false so code only runs once
            questComplete = false;

            // Hardcoded solution to activate quest givers
            switch (currentQuest.questName) 
            {
                case "The first boss:":
                    questGivers[1].gameObject.SetActive(true);
                    break;
                case "The second boss:":
                    questGivers[2].gameObject.SetActive(true);
                    break;
                case "The third boss:":
                    questGivers[3].gameObject.SetActive(true);
                    break;
                case "Nightshade grunts:":
                    questGivers[4].gameObject.SetActive(true);
                    break;
                case "The Final Boss:":
                    questGivers[5].gameObject.SetActive(true);
                    break;
            }
                

            if (currentQuest.nextQuest != null)
            {
                // If there is a next quest, start it
                currentQuest.nextQuest.StartQuest(this);
            }
            else
            {
                // If there isn't a next quest, clear the quest slot
                currentQuest = null;
            }

            StartCoroutine(showQuestCompleteMessage());
        }
    }

    public IEnumerator showQuestCompleteMessage() 
    {
        // Activate the quest complete message for a second
        questCompleteUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        questCompleteUI.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentQuest.targetLocation, playerDetectionRadius);
    }
}
