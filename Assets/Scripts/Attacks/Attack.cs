using UnityEngine;

public abstract class Attack : ScriptableObject {

    public readonly float attackRate;
    public float damage;

    public abstract void OnAttack(Hero hero);
}
