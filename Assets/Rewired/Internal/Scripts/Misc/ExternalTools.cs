#if UNITY_6 || UNITY_7 || UNITY_8 || UNITY_9 || UNITY_10 || UNITY_2017 || UNITY_2018 || UNITY_2019 || UNITY_2020
#define UNITY_6_PLUS
#endif

#if UNITY_5 || UNITY_6_PLUS
#define UNITY_5_PLUS
#endif

#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_1_PLUS
#endif

#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_2_PLUS
#endif

#if UNITY_5_3_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_3_PLUS
#endif

#if UNITY_5_4_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_4_PLUS
#endif

#if UNITY_5_5_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_5_PLUS
#endif

#if UNITY_5_6_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_6_PLUS
#endif

#if UNITY_5_7_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_7_PLUS
#endif

#if UNITY_5_8_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_8_PLUS
#endif

#if UNITY_5_9_OR_NEWER || UNITY_6_PLUS
#define UNITY_5_9_PLUS
#endif

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_PLUS
#define SUPPORTS_UNITY_UI
#endif

// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649
#pragma warning disable 0067

namespace Rewired.Utils {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Rewired.Utils.Interfaces;

    /// <exclude></exclude>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ExternalTools : IExternalTools {

        public object GetPlatformInitializer() {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Rewired.Utils.Platforms.WebGL.Main.GetPlatformInitializer();
#else
            return null;
#endif
        }
        
        public string GetFocusedEditorWindowTitle() {
#if UNITY_EDITOR
            UnityEditor.EditorWindow window = UnityEditor.EditorWindow.focusedWindow;
            return window != null ? window.title : string.Empty;
#else
            return string.Empty;
#endif
        }

        // Linux Tools
#if UNITY_5_PLUS && UNITY_STANDALONE_LINUX
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
#if !REWIRED_XBOXONE_DISABLE_VIBRATION
            try {
                Ext_Gamepad_UpdatePlugin();
            } catch {
            }
#endif
        }

        public bool XboxOne_Gamepad_SetGamepadVibration(ulong xboxOneJoystickId, float leftMotor, float rightMotor, float leftTriggerLevel, float rightTriggerLevel) {
#if !REWIRED_XBOXONE_DISABLE_VIBRATION
            try {
                return Ext_Gamepad_SetGamepadVibration(xboxOneJoystickId, leftMotor, rightMotor, leftTriggerLevel, rightTriggerLevel);
            } catch {
                return false;
            }
#else
            return false;
#endif
        }

        public void XboxOne_Gamepad_PulseVibrateMotor(ulong xboxOneJoystickId, int motorInt, float startLevel, float endLevel, ulong durationMS) {
#if !REWIRED_XBOXONE_DISABLE_VIBRATION
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
#endif
        }

#if !REWIRED_XBOXONE_DISABLE_VIBRATION

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

#endif
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

#if UNITY_PS4

        public Vector3 PS4Input_GetLastAcceleration(int id) {
            return UnityEngine.PS4.PS4Input.GetLastAcceleration(id);
        }

        public Vector3 PS4Input_GetLastGyro(int id) {
            return UnityEngine.PS4.PS4Input.GetLastGyro(id);
        }

        public Vector4 PS4Input_GetLastOrientation(int id) {
            return UnityEngine.PS4.PS4Input.GetLastOrientation(id);
        }

        public void PS4Input_GetLastTouchData(int id, out int touchNum, out int touch0x, out int touch0y, out int touch0id, out int touch1x, out int touch1y, out int touch1id) {
            UnityEngine.PS4.PS4Input.GetLastTouchData(id, out touchNum, out touch0x, out touch0y, out touch0id, out touch1x, out touch1y, out touch1id);
        }

        public void PS4Input_GetPadControllerInformation(int id, out float touchpixelDensity, out int touchResolutionX, out int touchResolutionY, out int analogDeadZoneLeft, out int analogDeadZoneright, out int connectionType) {
            UnityEngine.PS4.PS4Input.ConnectionType connectionTypeEnum;
            UnityEngine.PS4.PS4Input.GetPadControllerInformation(id, out touchpixelDensity, out touchResolutionX, out touchResolutionY, out analogDeadZoneLeft, out analogDeadZoneright, out connectionTypeEnum);
            connectionType = (int)connectionTypeEnum;
        }

        public void PS4Input_PadSetMotionSensorState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.PadSetMotionSensorState(id, bEnable);
        }

        public void PS4Input_PadSetTiltCorrectionState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.PadSetTiltCorrectionState(id, bEnable);
        }

        public void PS4Input_PadSetAngularVelocityDeadbandState(int id, bool bEnable) {
            UnityEngine.PS4.PS4Input.PadSetAngularVelocityDeadbandState(id, bEnable);
        }

        public void PS4Input_PadSetLightBar(int id, int red, int green, int blue) {
            UnityEngine.PS4.PS4Input.PadSetLightBar(id, red, green, blue);
        }

        public void PS4Input_PadResetLightBar(int id) {
            UnityEngine.PS4.PS4Input.PadResetLightBar(id);
        }

        public void PS4Input_PadSetVibration(int id, int largeMotor, int smallMotor) {
            UnityEngine.PS4.PS4Input.PadSetVibration(id, largeMotor, smallMotor);
        }

        public void PS4Input_PadResetOrientation(int id) {
            UnityEngine.PS4.PS4Input.PadResetOrientation(id);
        }

        public bool PS4Input_PadIsConnected(int id) {
            return UnityEngine.PS4.PS4Input.PadIsConnected(id);
        }

        public object PS4Input_PadGetUsersDetails(int slot) {
            UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.PadGetUsersDetails(slot);
            return new Rewired.Platforms.PS4.LoggedInUser() {
                status = user.status,
                primaryUser = user.primaryUser,
                userId = user.userId,
                color = user.color,
                userName = user.userName,
                padHandle = user.padHandle,
                move0Handle = user.move0Handle,
                move1Handle = user.move1Handle
            };
        }

		public Vector3 PS4Input_GetLastMoveAcceleration(int id, int index) {
            return UnityEngine.PS4.PS4Input.GetLastMoveAcceleration(id, index);
        }

        public Vector3 PS4Input_GetLastMoveGyro(int id, int index) {
            return UnityEngine.PS4.PS4Input.GetLastMoveGyro(id, index);
        }

        public int PS4Input_MoveGetButtons(int id, int index) {
            return UnityEngine.PS4.PS4Input.MoveGetButtons(id, index);
        }

        public int PS4Input_MoveGetAnalogButton(int id, int index) {
            return UnityEngine.PS4.PS4Input.MoveGetAnalogButton(id, index);
        }

        public bool PS4Input_MoveIsConnected(int id, int index) {
            return UnityEngine.PS4.PS4Input.MoveIsConnected(id, index);
        }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles, int[] secondaryHandles) {
            return UnityEngine.PS4.PS4Input.MoveGetUsersMoveHandles(maxNumberControllers, primaryHandles, secondaryHandles);
        }

		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles) {
			return UnityEngine.PS4.PS4Input.MoveGetUsersMoveHandles(maxNumberControllers, primaryHandles);
		}

		public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers) {
			return UnityEngine.PS4.PS4Input.MoveGetUsersMoveHandles(maxNumberControllers);
		}

		public System.IntPtr PS4Input_MoveGetControllerInputForTracking() {
            return UnityEngine.PS4.PS4Input.MoveGetControllerInputForTracking();
        }

#else
        public Vector3 PS4Input_GetLastAcceleration(int id) { return Vector3.zero; }

        public Vector3 PS4Input_GetLastGyro(int id) { return Vector3.zero; }

        public Vector4 PS4Input_GetLastOrientation(int id) { return Vector4.zero; }

        public void PS4Input_GetLastTouchData(int id, out int touchNum, out int touch0x, out int touch0y, out int touch0id, out int touch1x, out int touch1y, out int touch1id) { touchNum = 0; touch0x = 0; touch0y = 0; touch0id = 0; touch1x = 0; touch1y = 0; touch1id = 0; }

        public void PS4Input_GetPadControllerInformation(int id, out float touchpixelDensity, out int touchResolutionX, out int touchResolutionY, out int analogDeadZoneLeft, out int analogDeadZoneright, out int connectionType) { touchpixelDensity = 0f; touchResolutionX = 0; touchResolutionY = 0; analogDeadZoneLeft = 0; analogDeadZoneright = 0; connectionType = 0; }

        public void PS4Input_PadSetMotionSensorState(int id, bool bEnable) { }

        public void PS4Input_PadSetTiltCorrectionState(int id, bool bEnable) { }

        public void PS4Input_PadSetAngularVelocityDeadbandState(int id, bool bEnable) { }

        public void PS4Input_PadSetLightBar(int id, int red, int green, int blue) { }

        public void PS4Input_PadResetLightBar(int id) { }

        public void PS4Input_PadSetVibration(int id, int largeMotor, int smallMotor) { }

        public void PS4Input_PadResetOrientation(int id) { }

        public bool PS4Input_PadIsConnected(int id) { return false; }

        public object PS4Input_PadGetUsersDetails(int slot) { return null; }

        public Vector3 PS4Input_GetLastMoveAcceleration(int id, int index) { return Vector3.zero; }

        public Vector3 PS4Input_GetLastMoveGyro(int id, int index) { return Vector3.zero; }

        public int PS4Input_MoveGetButtons(int id, int index) { return 0; }

        public int PS4Input_MoveGetAnalogButton(int id, int index) { return 0; }

        public bool PS4Input_MoveIsConnected(int id, int index) { return false; }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles, int[] secondaryHandles) { return 0; }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers, int[] primaryHandles) { return 0; }

        public int PS4Input_MoveGetUsersMoveHandles(int maxNumberControllers) { return 0; }

        public System.IntPtr PS4Input_MoveGetControllerInputForTracking() { return System.IntPtr.Zero; }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR

        const int SDK_VERSION_HONEYCOMB = 9;
        const int SDK_VERSION_KITKAT = 19;

        public void GetDeviceVIDPIDs(out List<int> vids, out List<int> pids) {

            vids = new List<int>();
            pids = new List<int>();

            try {
                // Get the Android SDK version
                int androidSDKVersion = SDK_VERSION_HONEYCOMB;
                using(var version = new AndroidJavaClass("android.os.Build$VERSION")) {
                    androidSDKVersion = version.GetStatic<int>("SDK_INT");
                }
                if(androidSDKVersion < SDK_VERSION_KITKAT) return;

                AndroidJavaClass android_view_InputDevice = new AndroidJavaClass("android.view.InputDevice");

                int[] ids = null;
                using(AndroidJavaObject jniArray = android_view_InputDevice.CallStatic<AndroidJavaObject>("getDeviceIds")) {
                    if(jniArray != null) {
                        ids = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jniArray.GetRawObject());
                    }
                }

                if(ids == null) return;
                for(int i = 0; i < ids.Length; i++) {
                    try {
                        using(AndroidJavaObject jo = android_view_InputDevice.CallStatic<AndroidJavaObject>("getDevice", ids[i])) {
                            if(jo == null) continue;
                            vids.Add(jo.Call<int>("getVendorId"));
                            pids.Add(jo.Call<int>("getProductId"));
                        }
                    } catch {
                    }
                }
            } catch {
            }
        }
#else
        public void GetDeviceVIDPIDs(out List<int> vids, out List<int> pids) {
            vids = new List<int>();
            pids = new List<int>();
        }
#endif
        #region Unity UI


#if SUPPORTS_UNITY_UI

        public bool UnityUI_Graphic_GetRaycastTarget(object graphic) {
            if(graphic as UnityEngine.UI.Graphic == null) return false;
#if UNITY_5_2_PLUS
            return (graphic as UnityEngine.UI.Graphic).raycastTarget;
#else
            return true;
#endif
        }
        public void UnityUI_Graphic_SetRaycastTarget(object graphic, bool value) {
            if(graphic as UnityEngine.UI.Graphic == null) return;
#if UNITY_5_2_PLUS
            (graphic as UnityEngine.UI.Graphic).raycastTarget = value;
#endif
        }
#else
        public bool UnityUI_Graphic_GetRaycastTarget(object graphic) { return true; }
        public void UnityUI_Graphic_SetRaycastTarget(object graphic, bool value) { }
#endif

        #endregion
        
        #region Touch
        
        public bool UnityInput_IsTouchPressureSupported {
          get {
#if UNITY_5_3_PLUS
              return UnityEngine.Input.touchPressureSupported;
#else
              return false;
#endif
          }
        }
        
        public float UnityInput_GetTouchPressure(ref UnityEngine.Touch touch) {
#if UNITY_5_3_PLUS
            return touch.pressure;
#else
            return touch.phase != UnityEngine.TouchPhase.Ended &&
                touch.phase != UnityEngine.TouchPhase.Canceled
                ? 1.0f : 0.0f;
#endif
        }
        
        public float UnityInput_GetTouchMaximumPossiblePressure(ref UnityEngine.Touch touch) {
#if UNITY_5_3_PLUS
            return touch.maximumPossiblePressure;
#else
            return 1.0f;
#endif
        }
        
        #endregion
    }
}