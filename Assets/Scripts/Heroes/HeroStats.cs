using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Hero Stats")]
public class HeroStats : ScriptableObject
{
    public int maxHealth;
    public float attackSpeed = 1.2f;
    public float rangedFireRate = 0.4f;
}
