using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class PlayerStats
{
    public int healthPoints;
    public int toughness;
    public int attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float movementSpeed;

    public PlayerStats( int healthPoints,
                        int toughness,
                        int attackDamage,
                        float attackSpeed,
                        float attackRange,
                        float movementSpeed)
    {
        this.healthPoints = healthPoints;
        this.toughness = toughness;
        this.attackDamage = attackDamage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
        this.movementSpeed = movementSpeed;
    }

}