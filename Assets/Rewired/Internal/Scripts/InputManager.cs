// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired {

    using UnityEngine;
    using System.Collections.Generic;
    using Rewired.Platforms;
    using Rewired.Utils;
    using Rewired.Utils.Interfaces;

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class InputManager : InputManager_Base {

        protected override void DetectPlatform() {
            // Set the editor and platform versions

            editorPlatform = EditorPlatform.None;
            platform = Platform.Unknown;
            webplayerPlatform = WebplayerPlatform.None;
            isEditor = false;
            string deviceName = SystemInfo.deviceName ?? string.Empty;
            string deviceModel = SystemInfo.deviceModel ?? string.Empty;

#if UNITY_EDITOR
            isEditor = true;
            editorPlatform = EditorPlatform.Unknown;
#endif

#if UNITY_EDITOR_WIN
            editorPlatform = EditorPlatform.Windows;
#endif

#if UNITY_EDITOR_LINUX
            editorPlatform = EditorPlatform.Linux;
#endif

#if UNITY_EDITOR_OSX
            editorPlatform = EditorPlatform.OSX;
#endif

#if UNITY_STANDALONE_OSX
            platform = Platform.OSX;
#endif

#if UNITY_DASHBOARD_WIDGET

#endif

#if UNITY_STANDALONE_WIN
            platform = Platform.Windows;
#endif

#if UNITY_STANDALONE_LINUX
            platform = Platform.Linux;
#endif

#if UNITY_ANDROID
            platform = Platform.Android;
#if !UNITY_EDITOR
            // Handle special Android platforms
            if(CheckDeviceName("OUYA", deviceName, deviceModel)) {
                platform = Platform.Ouya;
            } else if(CheckDeviceName("Amazon AFT.*", deviceName, deviceModel)) {
                platform = Platform.AmazonFireTV;
            } else if(CheckDeviceName("razer Forge", deviceName, deviceModel)) {
#if REWIRED_OUYA && REWIRED_USE_OUYA_SDK_ON_FORGETV
                platform = Platform.Ouya;
#else
                platform = Platform.RazerForgeTV;
#endif
            }
#endif
#endif

#if UNITY_BLACKBERRY
            platform = Platform.Blackberry;
#endif

#if UNITY_IPHONE || UNITY_IOS
            platform = Platform.iOS;
#endif

#if UNITY_TVOS
            platform = Platform.tvOS;
#endif

#if UNITY_PS3
            platform = Platform.PS3;
#endif

#if UNITY_PS4
            platform = Platform.PS4;
#endif

#if UNITY_PSP2
            platform = Platform.PSVita;
#endif

#if UNITY_PSM
            platform = Platform.PSMobile;
#endif

#if UNITY_XBOX360
            platform = Platform.Xbox360;
#endif

#if UNITY_XBOXONE
            platform = Platform.XboxOne;
#endif

#if UNITY_WII
            platform = Platform.Wii;
#endif

#if UNITY_WIIU
            platform = Platform.WiiU;
#endif

#if UNITY_N3DS
            platform = Platform.N3DS;
#endif

#if UNITY_SWITCH
            platform = Platform.Switch;
#endif

#if UNITY_FLASH
            platform = Platform.Flash;
#endif

#if UNITY_METRO || UNITY_WSA || UNITY_WSA_8_0
            platform = Platform.WindowsAppStore;
#endif

#if UNITY_WSA_8_1
            platform = Platform.Windows81Store;
#endif

            // Windows 8.1 Universal
#if UNITY_WINRT_8_1 && !UNITY_WSA_8_1 // this seems to be the only way to detect this
    platform = Platform.Windows81Store;
#endif

            // Windows Phone overrides Windows Store -- this is not set when doing Universal 8.1 builds
#if UNITY_WP8 || UNITY_WP8_1 || UNITY_WP_8 || UNITY_WP_8_1 // documentation error on format of WP8 defines, so include both
            platform = Platform.WindowsPhone8;
#endif

#if UNITY_WSA_10_0
            platform = Platform.WindowsUWP;
#endif

#if UNITY_WEBGL
            platform = Platform.WebGL;
#endif

            // Check if Webplayer
#if UNITY_WEBPLAYER

            webplayerPlatform = UnityTools.DetermineWebplayerPlatformType(platform, editorPlatform); // call this BEFORE you change the platform so we still know what base system this is
            platform = Platform.Webplayer;

#endif
        }

        protected override void CheckRecompile() {
#if UNITY_EDITOR
            // Destroy system if recompiling
            if(UnityEditor.EditorApplication.isCompiling) { // editor is recompiling
                if(!isCompiling) { // this is the first cycle of recompile
                    isCompiling = true; // flag it
                    RecompileStart();
                }
                return;
            }

            // Check for end of compile
            if(isCompiling) { // compiling is done
                isCompiling = false; // flag off
                RecompileEnd();
            }
#endif
        }

        protected override IExternalTools GetExternalTools() {
            return new ExternalTools();
        }

        private bool CheckDeviceName(string searchPattern, string deviceName, string deviceModel) {
            return System.Text.RegularExpressions.Regex.IsMatch(deviceName, searchPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                System.Text.RegularExpressions.Regex.IsMatch(deviceModel, searchPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}