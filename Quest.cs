using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Quest")]
// Allows me to create a new scriptable object of this type in assets
public class Quest : ScriptableObject
{
    public string questName;
    public string questDescription;
    public questType type;
    public Vector3 targetLocation;
    public int targetKillCount;
    public EnemyHealth enemy;
    public Quest nextQuest;

    public enum questType
    {
        kill,
        // For enemy killing quests
        travel
        // For going to new areas
    }

    public void StartQuest(QuestSlot questSlot) 
    {
        questSlot.currentQuest = this;
        // Called from quest giver scripts that activate the quest
        // and start recording progress and add it to the player's quest slot
    }
}   
