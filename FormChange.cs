using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FormChange : MonoBehaviour
{
    [HideInInspector] public Form equippedForm;
    // Stores the form the player has currently equipped
    private bool formActivated;
    private bool formDeactivated;
    public Attack attack;
    // Needed so the players attack damage can change depending on the form
    public Health health;
    // Needed so the players defence can change depending on the form
    public PlayerMovement pm;
    // Needed so the players speed can change depending on the form
    public float previousWalkSpeed;
    public float previousSprintSpeed;
    public float previousWallRunSpeed;
    public float previousDashSpeed;
    public bool formChanged;
    private bool IsChangingForm;

    [Header("Keybinds")]
    public KeyCode activateForm = KeyCode.X;
    public KeyCode deactivateForm = KeyCode.Z;

    [Header("Form Change logic")]
    public Camera combatCam;
    public Camera formChangeCam;
    public AudioSource auraSound;
    public AudioSource burstSound;
    public AudioClip powerDownSound;

    [Header("Form timer")]
    public float formTimer;
    public bool formTimerNeedsToBeOn;

    [Header("Form charging")]
    public float formProgress;
    public bool formCharged;

    [Header("Form change GUI")]
    public Image timerFill;
    public Image chargeFill;
    public TextMeshProUGUI Z;
    public TextMeshProUGUI X;
    public TextMeshProUGUI percentage;
    public GameObject formChangeProgressUI;
    public Canvas HUD;
    public Canvas formCamUI;

    [Header("Animations")]
    public Animator animator;

    public void Start() 
    {
        formProgress = 0;
        percentage.gameObject.SetActive(true);
        X.gameObject.SetActive(false);
        Z.gameObject.SetActive(false);
        chargeFill.gameObject.SetActive(true);
        timerFill.gameObject.SetActive(false);
    }
    
    public void Update()
    {
        ChangeForm();
        DeactivateForm();
        StartFormTimer();
        CheckIfFormCharged();

        // If there is no form equipped, hide the UI
        if (equippedForm == null)
            formChangeProgressUI.SetActive(false);
        // Otherwise show the UI
        else
            formChangeProgressUI.SetActive(true);
    }

    public void ChangeForm() 
    {
        formActivated = Input.GetKeyDown(activateForm);
        // Returns true if the activateForm (X) key is pressed

        // If the player does have a form equipped
        if (equippedForm != null)
        {
            // If the player has pressed the right key and they haven't already changed form
            if (formActivated && formChanged == false && formCharged && attack.readyToAttack == true)
            {
                // Deactivate the normal camera
                combatCam.gameObject.SetActive(false);
                // Activate the form change camera so the form change is clearly shown
                formChangeCam.gameObject.SetActive(true);
                
                // Activate the form aura
                equippedForm.formAura.gameObject.SetActive(true);

                // Store the players previous speeds in temp variables so that once the form is deactivated the speeds can be reset to their previous values
                previousWalkSpeed = pm.walkSpeed;
                previousSprintSpeed = pm.sprintSpeed;
                previousWallRunSpeed = pm.wallRunSpeed;
                previousDashSpeed = pm.dashSpeed;

                // Change the players speeds
                pm.walkSpeed += equippedForm.speed;
                pm.sprintSpeed += equippedForm.speed;
                pm.wallRunSpeed += equippedForm.speed;
                pm.dashSpeed += equippedForm.speed;

                // Change player attack
                attack.formAttack = equippedForm.attack;

                // Freeze players position so that they can't move during transformation
                pm.freeze = true;
                
                // Needed so that the player can't activate the form again
                formChanged = true;
                IsChangingForm = true;
                
                // Set the audio sources to the respective burst and aura sounds stored in the form
                burstSound.clip = equippedForm.burst;
                auraSound.clip = equippedForm.aura;
                // Play the burst sound
                burstSound.Play();

                // Start the transforming animation
                animator.SetBool("IsTransforming", true);

                // Set to false so player can't transform again after they have done it once already
                formCharged = false;

                // Reset form progress
                formProgress = 0;

                // Change UI
                HUD.gameObject.SetActive(false);
                formCamUI.gameObject.SetActive(true);
                timerFill.gameObject.SetActive(true);
                chargeFill.gameObject.SetActive(false);
                X.gameObject.SetActive(false);
                Z.gameObject.SetActive(true);

                // Coroutine used as delay is needed in order to play transform animation and to play the aura sound
                StartCoroutine(EndTransition());
            }
        }
    }

    public void DeactivateForm() 
    {
        formDeactivated = Input.GetKeyDown(deactivateForm);
        // Returns true if the deactivateForm (Z) key is pressed

        // If the player has pressed the right key or the form timer runs out and they have already changed form
        if ((formDeactivated || formTimer <= 0) && formChanged == true && !IsChangingForm)
        {
            // deactivate the form aura
            equippedForm.formAura.gameObject.SetActive(false);
            // Reset the speeds
            pm.walkSpeed = previousWalkSpeed;
            pm.sprintSpeed = previousSprintSpeed;
            pm.wallRunSpeed = previousWallRunSpeed;
            pm.dashSpeed = previousDashSpeed;

            // Reset players attack
            attack.formAttack = 0;

            // Set the burst audio source clip to the power down sound
            burstSound.clip = powerDownSound;
            // Play the power down sound
            burstSound.Play();
            // Stop the aura sound from playing any longer
            auraSound.Stop();
            
            // Set to false so that the player can change form again
            formChanged = false;

            // Stop the form timer
            formTimerNeedsToBeOn = false;
            // Reset the form timer
            formTimer = 90;
            // Disable the timer UI
            timerFill.gameObject.SetActive(false);
            // Reset the timer UI
            timerFill.fillAmount = 1;

            // Change UI
            Z.gameObject.SetActive(false);
            percentage.gameObject.SetActive(true);
            chargeFill.gameObject.SetActive(true);
        }
    }

    public IEnumerator EndTransition()
    {
        // Wait the length of time the burst sound plays, -1 needed as sounds have quiet section for 1 second at the end
        yield return new WaitForSeconds(equippedForm.burst.length - 1);
        // Reset the cameras so that the player has the right camera again
        combatCam.gameObject.SetActive(true);
        formChangeCam.gameObject.SetActive(false);

        // Play the aura sound after the transformation is complete
        auraSound.Play();
        // Stop the transforming animation so the player can move again and is back in the idle state
        animator.SetBool("IsTransforming", false);

        IsChangingForm = false;

        // Set to false so player can move again after transformation
        pm.freeze = false;

        // Change UI back
        HUD.gameObject.SetActive(true);
        formCamUI.gameObject.SetActive(false);

        // Start form timer
        formTimerNeedsToBeOn = true;
    }

    public void StartFormTimer() 
    {
        // If timer needs to be on
        if (formTimerNeedsToBeOn)
        {
            // Subtract time from timer
            formTimer -= Time.deltaTime;
            /* Reflect timer in UI. Converted to a decimal first as fill 
               amount property can only take a value between 0 and 1 */
            timerFill.fillAmount = (((formTimer)/90));
        }
    }

    public void CheckIfFormCharged() 
    {
        // Update fill UI to show how many hits more the player needs to fully charge form
        chargeFill.fillAmount = formProgress;
        // Show in text form as well. Formatted to 1 d.p.
        percentage.text = (formProgress * 100).ToString("F1") + "%";
        
        // If the form progress is 1, it has been fully charged
        if (formProgress == 1) 
        {
            formCharged = true;
        }

        // If form is charged
        if (formCharged)
        {
            // Disable percentage text
            percentage.gameObject.SetActive(false);
            // Enable X text so player knows to press X to activate form
            X.gameObject.SetActive(true);
        }
    }

    public void DeactivateIfDead() 
    {
        if (formChanged == true && !IsChangingForm) 
        {
            // deactivate the form aura
            equippedForm.formAura.gameObject.SetActive(false);
            // Reset the speeds
            pm.walkSpeed = previousWalkSpeed;
            pm.sprintSpeed = previousSprintSpeed;
            pm.wallRunSpeed = previousWallRunSpeed;
            pm.dashSpeed = previousDashSpeed;

            // Reset players attack
            attack.formAttack = 0;

            // Set the burst audio source clip to the power down sound
            burstSound.clip = powerDownSound;
            // Play the power down sound
            burstSound.Play();
            // Stop the aura sound from playing any longer
            auraSound.Stop();

            // Set to false so that the player can change form again
            formChanged = false;

            // Stop the form timer
            formTimerNeedsToBeOn = false;
            // Reset the form timer
            formTimer = 90;
            // Disable the timer UI
            timerFill.gameObject.SetActive(false);
            // Reset the timer UI
            timerFill.fillAmount = 1;

            // Change UI
            Z.gameObject.SetActive(false);
            percentage.gameObject.SetActive(true);
            chargeFill.gameObject.SetActive(true);
        }
    }
}
