using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler onDead;

    [SerializeField] private int health = 100;
    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            health = 0;

        if (health == 0)
            Die();
    }
    private void Die()
    {
        onDead?.Invoke(this,EventArgs.Empty);
    }
}
