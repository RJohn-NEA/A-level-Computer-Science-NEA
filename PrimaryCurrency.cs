using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryCurrency : MonoBehaviour
{
    public float balance;

    public void ReduceBalance(float amount) 
    {
        balance -= amount;
    }

    public void AddToBalance(float amount) 
    { 
        balance += amount;
    }
}
