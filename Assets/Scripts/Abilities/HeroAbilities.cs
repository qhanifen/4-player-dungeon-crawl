using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Hero/Abilities List")]
public class HeroAbilities : ScriptableObject
{
    public Attack basicAttack;
    public Ability[] abilities;
}
