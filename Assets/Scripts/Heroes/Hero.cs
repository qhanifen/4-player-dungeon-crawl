using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
public class Hero : MonoBehaviour, ITargetable, IDamageable, IHealable {
    
    public PlayerController controller;

    public HeroStats stats;
    public HeroAbilities abilities;

	public string heroName;
	public int health = 100;	

	public float attackTimer = 0.0f;

	public Transform firePoint;
	public Projectile defaultShot;

	public Transform target = null;

    // Use this for initialization
    void Start ()
	{
        controller = GetComponent<PlayerController>();
	}

	public void Attack()
	{
        abilities.basicAttack.OnAttack(this);
	}

	#region ITargetable implementation

	public void OnTargeted ()
	{
		throw new NotImplementedException ();
	}

	public Transform GetTarget
	{
        get
        {
            return this.transform;
        }
	}

	#endregion

	#region IDamageable implementation

	public void TakeDamage (int damage)
	{
		health = Mathf.Clamp(health - damage, 0, health);
		if(health <= 0)
		{
			Die();
		}
	}

	public void Die()
	{

	}

	#endregion

	#region IHealable implementation

	public void Heal (int healAmount)
	{
		health = Mathf.Clamp(health + healAmount, 0, stats.maxHealth);
	}

    public void OnUntargeted()
    {
        throw new NotImplementedException();
    }

    #endregion
}
