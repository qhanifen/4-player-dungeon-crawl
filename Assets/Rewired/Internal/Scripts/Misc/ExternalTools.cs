// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Utils {

    using UnityEngine;
    using System.Collections;
    using Rewired.Utils.Interfaces;

    /// <exclude></exclude>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ExternalTools : IExternalTools {

        // Linux Tools
#if UNITY_5 && UNITY_STANDALONE_LINUX
        public bool LinuxInput_IsJoystickPreconfigured(string name) {
            return UnityEngine.Input.IsJoystickPreconfigured(name);
        }
#else
        public bool LinuxInput_IsJoystickPreconfigured(string name) {
            return false;
            
        }
#endif

        // Xbox One Tools

#if UNITY_XBOXONE

        public event System.Action<uint, bool> XboxOneInput_OnGamepadStateChange {
            add { XboxOneInput.OnGamepadStateChange += new XboxOneInput.OnGamepadStateChangeEvent(value); }
            remove { XboxOneInput.OnGamepadStateChange -= new XboxOneInput.OnGamepadStateChangeEvent(value); }
        }
        public int XboxOneInput_GetUserIdForGamepad(uint id) { return XboxOneInput.GetUserIdForGamepad(id); }
        public ulong XboxOneInput_GetControllerId(uint unityJoystickId) { return XboxOneInput.GetControllerId(unityJoystickId); }
        public bool XboxOneInput_IsGamepadActive(uint unityJoystickId) { return XboxOneInput.IsGamepadActive(unityJoystickId); }
        public string XboxOneInput_GetControllerType(ulong xboxControllerId) { return XboxOneInput.GetControllerType(xboxControllerId); }
        public uint XboxOneInput_GetJoystickId(ulong xboxControllerId) { return XboxOneInput.GetJoystickId(xboxControllerId); }
        public void XboxOne_Gamepad_UpdatePlugin() {
            try {
                Ext_Gamepad_UpdatePlugin();
            } catch {
            }
        }
        public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel) {
            try {
                return Ext_Gamepad_SetGamepadVibration(xboxOneJoystickId, leftMotor, rightMotor, leftTriggerLevel, rightTriggerLevel);
            } catch {
                return false;
            }
        }
        public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS) {
            Rewired.Platforms.XboxOne.XboxOneGamepadMotorType motor = (Rewired.Platforms.XboxOne.XboxOneGamepadMotorType)motorInt;
            try {
                switch(motor) {
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.LeftMotor:
                        Ext_Gamepad_PulseVibrateLeftMotor(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.RightMotor:
                        Ext_Gamepad_PulseVibrateRightMotor(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.LeftTriggerMotor:
                        Ext_Gamepad_PulseVibrateLeftTrigger(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    case Rewired.Platforms.XboxOne.XboxOneGamepadMotorType.RightTriggerMotor:
                        Ext_Gamepad_PulseVibrateRightTrigger(xboxOneJoystickId, startLevel, endLevel, durationMS);
                        break;
                    default: throw new System.NotImplementedException();
                }
            } catch {
            }
        }

        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "UpdatePlugin")]
        private static extern void Ext_Gamepad_UpdatePlugin();
        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "SetGamepadVibration")]
        private static extern bool Ext_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel);
        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsLeftMotor")]
        private static extern void Ext_Gamepad_PulseVibrateLeftMotor(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);
        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsRightMotor")]
        private static extern void Ext_Gamepad_PulseVibrateRightMotor(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);
        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsLeftTrigger")]
        private static extern void Ext_Gamepad_PulseVibrateLeftTrigger(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);
        [System.Runtime.InteropServices.DllImport("Gamepad", EntryPoint = "PulseGamepadsRightTrigger")]
        private static extern void Ext_Gamepad_PulseVibrateRightTrigger(ulong xboxOneJoystickId, float startLevel, float endLevel, ulong durationMS);

#else
        public event System.Action<uint, bool> XboxOneInput_OnGamepadStateChange;
        public int XboxOneInput_GetUserIdForGamepad(uint id) { return 0; }
        public ulong XboxOneInput_GetControllerId(uint unityJoystickId) { return 0; }
        public bool XboxOneInput_IsGamepadActive(uint unityJoystickId) { return false; }
        public string XboxOneInput_GetControllerType(ulong xboxControllerId) { return string.Empty; }
        public uint XboxOneInput_GetJoystickId(ulong xboxControllerId) { return 0; }
        public void XboxOne_Gamepad_UpdatePlugin() { }
        public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel) { return false; }
        public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS) { }
#endif
    }
}