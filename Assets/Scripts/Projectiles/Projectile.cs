using UnityEngine;

[RequireComponent(typeof(Collider), (typeof(Rigidbody)))]
public class Projectile : MonoBehaviour, IProjectile
{
    public ProjectilePath path;

    public enum TargetType
    {
        Player,
        Enemy
    }

    public enum MissleType
    {
        Projectile,
        Missle,
        Homing,
    }

    public MissleType missleType;
    public TargetType targetType;
    public float projectileSpeed = 2.5f;
    public float turnSpeed = 5.0f;


    public Transform currentTarget;


    // Use this for initialization
    void Start()
    {

    }

    void FixedUpdate()
    {
        Move();
    }

    // Update is called once per frame
    public void Move()
    {
        switch (missleType)
        {
            case MissleType.Projectile:
            case MissleType.Missle:
                break;
            case MissleType.Homing:
                if (currentTarget != null)
                {
                    Vector3 relativePos = currentTarget.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * Time.fixedDeltaTime);
                }
                break;
        }
        transform.Translate(Vector3.forward * projectileSpeed * Time.fixedDeltaTime, Space.Self);
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    public MissleType GetProjectileType()
    {
        return this.missleType;
    }

    public void SetProjectileType(MissleType type)
    {
        missleType = type;
    }

    public void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
