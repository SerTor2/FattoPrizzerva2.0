﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tasks;
using Dialogues;

public class TestPlayer : MonoBehaviour
{
    
    TasksManager tasksManager;
    public LocalDialogController dialogController;
    public int health;
    public int maxHealth;


    private void Start()
    {
        tasksManager = TasksManager.GetInstance();

        maxHealth = health = 100;


        
    }



    public void TakeDamage(int _damage)
    {
        health -= _damage;

        if (health <= 0)
            Debug.LogError("You are dead");
        else
            Debug.LogError("health = " + health);
    }
    public void TakeDamage(float _damagePerCent)
    {
        if (_damagePerCent < 0 || _damagePerCent > 1)
        {
            return;
        }

        int pureDamage;
        pureDamage = (int)(maxHealth * _damagePerCent);

        TakeDamage(pureDamage);

    }
    



}
