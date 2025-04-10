using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Exp : MonoBehaviour
{
    [Header("GUI")]
    public Slider slider;
    public TextMeshProUGUI tmp;

    [Header("Levels")]
    public int currentLevel;
    [HideInInspector] public int maximumlevel;
    public float currentEXP;
    public float maximumEXP;
    public float expMultipler = 1.1f;

    void Start()
    {
        slider.maxValue = maximumEXP;
        // Slider max value set to EXP points required to level up
        slider.value = currentEXP;
        // Set the GUI to show how much EXP points (set to 0 in inspector) the player currently has
        currentLevel = 0;
        // Set current level to 0 (will change in future to load previous level from a save file)
        maximumlevel = 100;
        // Set maximum level to 100
        tmp.text = currentLevel.ToString();
        // Shows the player current level in the GUI
    }

    void Update()
    {
        // Temporary for testing purposes
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            AddEXP(100000);
        }
    }

    public void AddEXP(float amount) 
    {
        float temp;
        float targetEXP;
        targetEXP = currentEXP + amount;


        if (targetEXP > maximumEXP)
        {
            // Temp variable to store EXP points needed to level up before leveling up to calculate excess exp to be added.
            temp = maximumEXP;
            LevelUp();
            // Workout excess EXP
            currentEXP = targetEXP - temp;
        }
        else if (targetEXP == maximumEXP) // If the player has enough EXP points to the next level
        {
            // Level up
            LevelUp();
        }
        else if (targetEXP < maximumEXP)
        {
            currentEXP = targetEXP;
        }

        // Update GUI to show new EXP level
        slider.value = currentEXP;
    }

    public void LevelUp() 
    {
        currentLevel += 1;
        //Add one to the players current level
        currentEXP = 0;
        //Reset their exp points
        slider.value = 0;
        //Reflect this in GUI
        maximumEXP *= expMultipler;
        //Make it harder to get to the next level by increasing exp points required
        slider.maxValue = maximumEXP;
        //Sets max value of the slider to the new amount of exp required
        tmp.text = currentLevel.ToString();
        // Reflect new level in GUI
    }
}
