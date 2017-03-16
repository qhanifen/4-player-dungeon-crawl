using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghoul : Enemy {

    [SerializeField]
    bool immune = true;
	
    //Override TakeDamage to check if Immune or not
    public override void TakeDamage(int damage)
    {
        if (immune)
        {
            return;
        }
        else
        {
            base.TakeDamage(damage);
        }
    }

    IEnumerator ImmuneTimer()
    {
        immune = false;
        float timer = 3f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        immune = true;
    }
    
    //Toggle immune on and off when attacking
    public override void Attack()
    {
        base.Attack();
        immune = false;

    }
    
}
