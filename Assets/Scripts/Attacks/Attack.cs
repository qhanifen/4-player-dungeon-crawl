using UnityEngine;

public abstract class Attack : ScriptableObject {

    public float attackRate;

    public abstract void ActivateAttack(Hero hero);
}
