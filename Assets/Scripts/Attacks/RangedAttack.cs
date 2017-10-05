using UnityEngine;

[CreateAssetMenu( menuName = "Hero/Attacks/Ranged Attack")]
public class RangedAttack : Attack {

    public Projectile projectile;

    public override void OnAttack(Hero hero)
    {
        FireProjectile(hero);
    }

    public void FireProjectile(Hero hero)
    {
        IProjectile shot = (Projectile)Instantiate(projectile, hero.firePoint.position, hero.firePoint.rotation);
        switch (shot.GetProjectileType())
        {
            case Projectile.MissleType.Missle:
            case Projectile.MissleType.Projectile:
                break;
            case Projectile.MissleType.Homing:
                if (hero.target != null)
                {
                    shot.SetTarget(hero.target);
                }
                break;
        }
    }
}
