// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.UI.ControlMapper {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using Rewired;

    public partial class ControlMapper {

        private enum LayoutElementSizeType {
            MinSize = 0,
            PreferredSize = 1
        }

        private enum WindowType {
            None = 0,
            ChooseJoystick,
            JoystickAssignmentConflict,
            ElementAssignment,
            ElementAssignmentPrePolling,
            ElementAssignmentPolling,
            ElementAssignmentResult,
            ElementAssignmentConflict,
            Calibration,
            CalibrateStep1,
            CalibrateStep2
        }
    }
}
