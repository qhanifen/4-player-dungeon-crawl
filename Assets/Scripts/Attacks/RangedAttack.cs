using UnityEngine;

[CreateAssetMenu( menuName = "Hero/Attacks/Ranged Attack")]
public class RangedAttack : Attack {

    public Projectile projectile;

    public override void ActivateAttack(Hero hero)
    {
        FireProjectile(hero);
    }

    public void FireProjectile(Hero hero)
    {
        Projectile p = Instantiate(projectile, hero.firePoint.position, Quaternion.identity, null);
    }
}
