using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Toggle State Controller")]
public class ToggleStateController : AIAction
{    
    public override void Act(StateController controller)
    {
        ToggleController(controller);
    }

    private void ToggleController(StateController controller)
    {
        controller.aiActive = !controller.aiActive;
    }
}
