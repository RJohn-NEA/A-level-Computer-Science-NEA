using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponBehaviour : MonoBehaviour
{
    private RangedWeapon rangedWeapon;
    [HideInInspector] public Vector3 instantiatePoint;
    private float distance;
    [HideInInspector] public float damage;
    [HideInInspector] public float range;

    
    private void Update()
    {
        distance = Vector3.Distance(this.transform.position, instantiatePoint);
        // If the distance between the prefab and the player is greater than the range
        if (distance > range)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the prefab collides with an enemy
        if (collision.gameObject.layer == 8)
        {
            Debug.Log("Has collided with enemy");
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                float targetHealth = enemyHealth.currentHealth - damage;

                if (targetHealth < 0)
                {
                    // Validation to see if health will go into negatives after attack
                    enemyHealth.currentHealth = 0;
                    // If it does, just set health to 0
                }
                else
                {
                    // Otherwise apply the damage to the object and deduct health points from it
                    enemyHealth.currentHealth = targetHealth;
                }
                Debug.Log(enemyHealth.currentHealth);
            }

            Destroy(this.gameObject);
        }
    }

}
