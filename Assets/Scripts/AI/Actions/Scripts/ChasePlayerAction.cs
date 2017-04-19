using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Chase Player")]
public class ChasePlayer : AIAction
{
    public override void Act(StateController controller)
    { 
        Chase(controller);
    }

    private void Chase(StateController controller)
    {
        //Transform target = CheckClosestTarget(controller);
        //controller.AI.target = target;        

    }

    /*private Transform CheckClosestTarget(StateController controller)
    {
        Transform target;
       

        //Save if MultiPath is too expensive
        /*foreach(Hero hero in GameManager.instance.heroes)
        {
            if(target != null)
            {
                float sqrDistance = Vector3.SqrMagnitude(hero.transform.position - controller.transform.position);
            }
            else
            {
                target = hero;
            }
        }
        return target;
    }*/
}
