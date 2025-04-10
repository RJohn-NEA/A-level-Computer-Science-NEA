using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    public string[] conversation1;
    private string[] currentConversation;
    public string characterName;
    public bool conversationActive;
    public int step;

    [Header("Dialogue UI")]
    public Canvas dialogue;
    public Canvas hud;
    public TextMeshProUGUI dialogueTextUI;
    public TextMeshProUGUI nameTextUI;

    [Header("Player Input")]
    public KeyCode continueConversation = KeyCode.Mouse0;
    public bool continueConversationPressed;

    [Header("Quest")]
    public Quest quest;
    public QuestSlot playerQuestSlot;


    [Header("References")]
    public Attack attack;
    public PlayerMovement pm;
    public GameObject combatCam;
    public Interactor interactor;

    private void Start()
    {
        // Set step to 0 so when conversation starts, it is at the beginning
        step = 0;
    }

    private void Update()
    {
        StartConversation(currentConversation);
    }

    public void Interact() 
    {
        // Show cursor again
        Cursor.visible = true;
        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        // Set conversation active bool in attack script so player can't attack
        attack.conversationActive = true;
        // Deactivate cinemachine component so camera can't be moved with cursor during conversation
        combatCam.SetActive(false);
        // Freeze the player so they can't move during the conversation
        pm.freeze = true;

        // Disable heads up display UI
        hud.gameObject.SetActive(false);
        // Enable dialogue UI
        dialogue.gameObject.SetActive(true);
        // Set the name text to the name of boss (set in inspector)
        nameTextUI.text = characterName + ":";

        // Let script know a conversation is currrently going on
        conversationActive = true;

        dialogueTextUI.text = conversation1[0];
        step++;
        Debug.Log("Step increased by 1 in interact (1)");
        currentConversation = conversation1;
    }

    public void StartConversation(string[] conversation)
    {
        continueConversationPressed = Input.GetKeyDown(continueConversation);

        // If a conversation is going on
        if (conversationActive)
        {
            // If the player presses the LMB to continue through the conversation
            if (continueConversationPressed)
            {
                // If the value of step does not exceed the length of the array
                if (step != conversation.Length)
                {
                    // Continue through the conversation
                    dialogueTextUI.text = conversation[step];
                }

                // Increment step so next round the next part of the conversation is played
                step++;
                Debug.Log("Step increased by 1 in conversation");

                // If the end of the conversation is reached
                if (step == conversation.Length + 1)
                {
                    // Let script know conversation is over
                    conversationActive = false;
                    // End interaction
                    interactor.IsInteracting = false;
                    // Reset the value of step
                    step = 0;
                    // Disable the dialogue UI
                    dialogue.gameObject.SetActive(false);
                    // Reenable the HUD UI;
                    hud.gameObject.SetActive(true);
                    // Hide the cursor
                    Cursor.visible = false;
                    // Lock the cursor to the middle of the screen
                    Cursor.lockState = CursorLockMode.Locked;
                    // Let attack script know player can attack again
                    attack.conversationActive = false;
                    // Let player move the camera again with their cursor
                    combatCam.SetActive(true);
                    // Let the player move again
                    pm.freeze = false;

                    if (quest != null)
                        quest.StartQuest(playerQuestSlot);
                        Destroy(this.gameObject);
                }
            }
        }
    }
}
