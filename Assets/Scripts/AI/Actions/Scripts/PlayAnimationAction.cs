using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Play Animation Action")]
public class PlayAnimationAction : AIAction
{
    public enum AnimationType
    {
        Boolean,
        Trigger,
        Float,
        Int
    }

    public AnimationType animationType;
    public string animationParameter;
    public bool toggle;
    public float amount;

    public override void Act(StateController controller)
    {
        PlayAnimation(controller);   
    }

    void PlayAnimation(StateController controller)
    {
        switch(animationType)
        {
            case AnimationType.Boolean:
                controller.AI.anim.SetBool(animationParameter, toggle);
                break;
            case AnimationType.Trigger:
                controller.AI.anim.SetBool(animationParameter, true);
                break;
            case AnimationType.Float:
                controller.AI.anim.SetFloat(animationParameter, amount);
                break;
            case AnimationType.Int:
                controller.AI.anim.SetInteger(animationParameter, (int)amount);
                break;
        }
    }
}
