using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class Enemy : AIBehavior, ITargetable, IDamageable
{
    public enum EnemyType
    {
        Melee,
        Archer,
        Minion,
        Caster,
        Spawner
    }

    public EnemyType enemyType;

    #region AI States

    #endregion

    //Old code, re
    /*
    switch(enemyType)
    {
        case EnemyType.Melee:
        case EnemyType.Minion:
            chaseState = new ChaseState(this);
            currentState = chaseState;
            break;
        case EnemyType.Archer:
        case EnemyType.Caster:
            positionState = new PositionState(this);
            currentState = positionState;
            break;
        case EnemyType.Spawner:
            roamingState = new RoamingState(this);
            currentState = roamingState;
            break;
        default:
            break;
    }
    */
    
    #region ITargetable implementation

    public Transform GetTarget
    {
        get
        {
            OnTargeted();
            return transform;
        }
    }

    public void OnTargeted()
    {
        //Turn on target reticle and set to colors of targeting players
    }
    
    public void OnUntargeted()
    {
        //Turn off target reticle
    }
    #endregion

    #region IDamageable implementation

    public virtual void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        //stateController.ToNextState();        
    }

    #endregion

    void OnDisable()
    {
        seeker.pathCallback -= OnPathComplete;
    }

    public void UpdateTarget(MultiTargetPath p)
    {
        //target = PlayerManager.GetHero(p.chosenTarget);
    }

    #region Combat Methods

    public virtual void Attack()
    {
        anim.SetTrigger("Attack");
    }

    #endregion    
}
