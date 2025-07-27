using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public bool isInvincible;

    public EventHandler<DamageArgs> OnDamageTaken;
    public EventHandler OnDeath;
    public EventHandler OnHealed;
    [SerializeField] Rigidbody2D rb;

    public bool cooldown;
    float timer;
    float timerGoal = .25f;

    private void Update()
    {
        if (cooldown)
        {
            timer += Time.deltaTime;
            if (timer > timerGoal)
            {
                cooldown = false;
            }
        }
    }

    public void SetHealth(int _val)
    {
        maxHealth = _val;
        currentHealth = _val;
    }

    public void SetCurrentHealth(int val)
    {
        currentHealth = val;
        OnHealed?.Invoke(this, System.EventArgs.Empty);
    }

    public void TakeDamage(int dmg, DamageArgs dmgArgs)
    {
        if (!isInvincible && !cooldown)
        {
            currentHealth -= dmg;
            OnDamageTaken?.Invoke(this, dmgArgs);

            if (currentHealth <= 0)
            {
                OnDeath?.Invoke(this, System.EventArgs.Empty);
            }

            var force = transform.position - dmgArgs.sender.position * 25;
            Debug.Log(force);
            rb.AddForce(force, ForceMode2D.Impulse);//ugh only works if controls are disabled because of rb.movePos... either make your own knockback or move rb with forces instead!
            cooldown = true;
            timer = 0f;
        }
    }

    public void RestoreHealth(int _newHealth)
    {
        currentHealth += _newHealth;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealed?.Invoke(this, EventArgs.Empty);
        cooldown = true;
        timer = 0f;
    }
}
