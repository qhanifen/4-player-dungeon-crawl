using UnityEngine;
using Pathfinding;

public class Imp : Enemy
{
    public override void StartPath()
    {
        Vector3[] pos = PlayerManager.instance.GetHeroPositions();
        path = seeker.StartMultiTargetPath(transform.position, pos, true);
    }

    public void GetClosestTargetFromPath()
    {
        MultiTargetPath p = (MultiTargetPath)path;        
        target = PlayerManager.GetHero(p.chosenTarget).transform;
    }
}
