using UnityEngine;
using System;

public class Hero : PlayerController, ITargetable, IDamageable, IHealable {
    
    public PlayerController controller;

    public HeroStats stats;
    public HeroAbilities abilities;

	public string heroName;
	public int health = 100;
    public float runSpeed = 10.0f;
    public float rotationSpeed = 8.0f;

    public float attackTimer = 0.0f;

	public Transform firePoint;
	public Projectile defaultShot;

	public Transform target = null;
        
    void Start ()
	{        
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

	public Transform Target
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
