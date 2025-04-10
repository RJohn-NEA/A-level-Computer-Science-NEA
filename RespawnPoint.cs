using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public Vector3 respawnPosition;
    public bool activated;
    private bool playerInRange;
    public float playerDetectionRadius;
    public LayerMask playerLayer;

    private void Update()
    {
        playerInRange = Physics.CheckSphere(respawnPosition, playerDetectionRadius, playerLayer);
        if (playerInRange) 
        {
            activated = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(respawnPosition, playerDetectionRadius);
    }
}
