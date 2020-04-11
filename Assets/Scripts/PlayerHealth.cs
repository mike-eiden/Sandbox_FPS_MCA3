using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthBar;
    public int maxHealth = 100;
    private int currentHealth;
    public LevelManager levelManager; 

    private void Start()
    {
        resetHelthBar();
    }


    private void Update()
    {
        if (transform.position.y < 0 && !levelManager.levelOverLock)
        {
            levelManager.levelLost();
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Enemy1"))
        {
            dealDamage(5);
        }
        else if (other.gameObject.tag.Equals("Enemy2"))
        {
            dealDamage(10);
        }
    }

    public void dealDamage(int amt)
    {
        if (!levelManager.levelOverLock)
        {
            if (currentHealth > 0)
            {
                currentHealth -= amt;
                healthBar.value = currentHealth;
            }
            else
            {
                levelManager.levelLost();
            }
        }
    }

    public void resetHelthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth; 
        currentHealth = maxHealth;
    }
}
