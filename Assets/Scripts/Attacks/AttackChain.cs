using UnityEngine;

[CreateAssetMenu(menuName = "Hero/Attacks/Attack Chain")]
public class AttackChain : Attack
{
    public int attackCounter = 0;
    public int attacksMax = 3;
    public float chainFallOff = 2.0f;
    
    public override void OnAttack(Hero hero)
    {           
        if (Time.time - hero.attackTimer > attackRate)
        {            
            return;
        }        
        else
        {
            if(Time.time - hero.attackTimer > chainFallOff || attackCounter >= attacksMax)
            {
                attackCounter = 0;
            }
            hero.attackTimer = Time.time;
            attackCounter++;
            hero.controller.anim.SetTrigger("Attack " + attackRate);
        }
    }
}
