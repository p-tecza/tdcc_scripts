using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CommonStats
{
    public int healthPoints { get; set; }
    public int toughness { get; set; }
    public int attackDamage { get; set; }
    public float attackSpeed { get; set; }
    public float attackRange { get; set; }
    public float movementSpeed { get; set; }
    public AttackType attackType { get; set; }
}