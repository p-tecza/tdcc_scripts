using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStats : CommonStats
{

    public PlayerStats( int healthPoints,
                        int toughness,
                        int attackDamage,
                        float attackSpeed,
                        float attackRange,
                        float movementSpeed,
                        AttackType attackType)
    {
        this.healthPoints = healthPoints;
        this.toughness = toughness;
        this.attackDamage = attackDamage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
        this.movementSpeed = movementSpeed;
        this.attackType = attackType;
    }

}