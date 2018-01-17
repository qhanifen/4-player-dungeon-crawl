using UnityEngine;

public interface ITargetable
{
    Transform Target { get; }
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
#region Internal
public interface ISystem
{
    bool Initialized { get; set; }
    void Initialize();
}
#endregion

#region AI 
public interface IChaser
{
    void FindClosestTarget();
    void ChaseTarget();
}
#endregion