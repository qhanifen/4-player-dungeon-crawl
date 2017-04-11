﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
public class Hero : MonoBehaviour, ITargetable, IDamageable, IHealable {

	[Serializable]
	public enum HeroType
	{
		Melee,
		Ranged
	};

	public HeroType heroType;

    public PlayerController controller;

    public HeroStats stats;
    public HeroAbilities abilities;

	public string heroName;
	public int health = 100;	

	private float timer = 0.0f;

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
		timer += Time.fixedDeltaTime;
		if(timer >= abilities.basicAttack.attackRate)
		{
			timer = 0.0f;

			IProjectile shot = (Projectile)Instantiate(defaultShot, firePoint.position, firePoint.rotation);
			switch (defaultShot.GetProjectileType())
			{
			case Projectile.MissleType.Missle: case Projectile.MissleType.Projectile:
				break;
			case Projectile.MissleType.Homing:
				if(target!=null)
				{	
					shot.SetTarget(target);
				}
				break;
			}


		}
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
