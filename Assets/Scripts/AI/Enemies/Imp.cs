using UnityEngine;
using Pathfinding;

public class Imp : Enemy
{
    void Awake()
    {
    }

    public override void StartPath()
    {
        seeker.StartMultiTargetPath(transform.position, PlayerManager.instance.GetHeroPositions(), true, OnPathComplete);
        GetTarget();
    }

    public override void GetTarget()
    {
        if (target == null)
        {
            base.GetTarget();
        }
        else
        {
            if (path != null)
            {
                MultiTargetPath p = (MultiTargetPath)path;
                target = GameManager.GetHero(p.chosenTarget).transform;
            }
            else
            {
                base.GetTarget();
            }
        }
    }
}
