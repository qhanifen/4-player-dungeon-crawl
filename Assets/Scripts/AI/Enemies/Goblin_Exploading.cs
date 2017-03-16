using System.Collections;
using UnityEngine;

public class Goblin_Exploding : Goblin, IExplodable
{
    [Header("Explosion Properties")]
    public Transform bombPosition;
    public float blastRadius = 5.0f;
    public float blastPower = 10f;
    public int blastDamage = 15;
    private float explodeTimer = 5.0f;

    #region IDamagable Implementation
    public override void Die()
    {
        base.Die();
        Explode();
    }
    #endregion

    #region IExplodable Implementation
    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(bombPosition.position, blastRadius);
        for(int i=0; i<colliders.Length; i++)
        {
            //To Do: Test performance to see if a better method to get and call Interface abilities
            //First, damage all Players and Enemies that are damageable
            IDamageable damageable = colliders[i].transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(blastDamage);
            }
            //Destroy all objects that are destructible
            IDestructible destructible = colliders[i].transform.GetComponent<IDestructible>();
            if(destructible != null)
            {
                destructible.Break();
            }
            //Do a Physics knockback from the explosion center
            colliders[i].GetComponent<Rigidbody>().AddExplosionForce(blastPower, bombPosition.position, blastRadius);
        }

        //Destroy Bomb
        GameObject.Destroy(bombPosition);
    }
    #endregion

    IEnumerator StartExplosionTimer()
    {
        while(explodeTimer > 0)
        {
            explodeTimer -= Time.deltaTime;
            yield return null;
        }
        Explode();
    }
}
