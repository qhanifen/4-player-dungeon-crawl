using UnityEngine;

public interface ITargetable
{
    Transform GetTarget { get; }
    void OnTargeted();
    void OnUntargeted();
}

public interface IDamageable
{
    void TakeDamage(int damage);
    void Die();
}

public interface IExplodable
{
    void Explode();
}    
public interface IDestructible
{
    void Break();
}

public interface ICastable
{
    void Cast();
}

public interface IHealable
{
    void Heal(int healAmount);
}

public interface IInteractible
{
    void OnTriggerEnter(Collider other);
    void OnTriggerExit(Collider other);
    void Activate();
}

public interface IProjectile
{
    void Move();
    Projectile.MissleType GetProjectileType();
    void SetProjectileType(Projectile.MissleType type);
    void SetTarget(Transform target);
    void OnTriggerEnter(Collider other);
}

