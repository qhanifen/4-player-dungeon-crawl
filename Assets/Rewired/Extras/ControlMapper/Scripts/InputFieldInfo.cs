// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Rewired;

    [AddComponentMenu("")]
    public class InputFieldInfo : UIElementInfo {
        public int actionId { get; set; }
        public AxisRange axisRange { get; set; }
        public int actionElementMapId { get; set; }
        public ControllerType controllerType { get; set; }
        public int controllerId { get; set; }
    }
}