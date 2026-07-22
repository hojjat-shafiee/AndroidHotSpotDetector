using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace AndroidHotspot
{
    /// <summary>
    /// Detects if the Android device is currently creating/providing a hotspot.
    /// Works with Android 4.0 (API 14) through Android 15 (API 35).
    /// </summary>
    public static class AndroidHotspotDetector
    {
        /// <summary>
        /// Detects if the Android device is currently creating/providing a hotspot.
        /// Uses a combination of WifiManager (hidden API) and ConnectivityManager (tethered interfaces).
        /// </summary>
        public static bool IsDeviceHotspotEnabled()
        {
#if UNITY_ANDROID
            try
            {
                // Method 1: Check tethered interfaces (most reliable for modern Android)
                if (IsAnyTetherActive()) return true;

                // Method 2: Check WifiManager state (hidden API, works on most devices)
                return IsWifiApEnabled();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AndroidHotspot] Error detecting hotspot: {ex.Message}");
                return false;
            }
#else
        return false;
#endif
        }

#if UNITY_ANDROID
        /// <summary>
        /// Modern API method using getWifiApState() - Works via reflection on most Android versions.
        /// On Android 10+ (API 29+), this might be restricted for non-system apps, 
        /// but Unity's JNI bridge usually handles the greylist access.
        /// </summary>
        private static bool IsWifiApEnabled()
        {
            try
            {
                using (AndroidJavaObject wifiManager = GetSystemService("wifi"))
                {
                    if (wifiManager == null) return false;
                    
                    // getWifiApState() returns 13 when enabled (WIFI_AP_STATE_ENABLED)
                    int state = wifiManager.Call<int>("getWifiApState");
                    return state == 13;
                }
            }
            catch (Exception)
            {
                // Silently return false if the hidden API call fails
                return false;
            }
        }

        /// <summary>
        /// Enhanced detection using ConnectivityManager - Works with all Android versions.
        /// Detects active tethering state (WiFi, USB, Bluetooth).
        /// </summary>
        public static bool IsAnyTetherActive()
        {
            try
            {
                using (AndroidJavaObject connectivityManager = GetSystemService("connectivity"))
                {
                    if (connectivityManager == null) return false;

                    // getTetheredIfaces() returns string[] of active tethered interfaces
                    string[] activeTethers = connectivityManager.Call<string[]>("getTetheredIfaces");
                    return activeTethers != null && activeTethers.Length > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Detailed hotspot state detection - Returns specific state.
        /// </summary>
        public static HotspotState GetHotspotState()
        {
#if UNITY_ANDROID
            try
            {
                using (AndroidJavaObject wifiManager = GetSystemService("wifi"))
                {
                    if (wifiManager == null) return HotspotState.Unknown;
                    int state = wifiManager.Call<int>("getWifiApState");
                    return (HotspotState)state;
                }
            }
            catch (Exception)
            {
                return HotspotState.Unknown;
            }
#else
        return HotspotState.Unknown;
#endif
        }

        /// <summary>
        /// Gets the Android system service by name.
        /// </summary>
        private static AndroidJavaObject GetSystemService(string serviceName)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return activity.Call<AndroidJavaObject>("getSystemService", serviceName);
            }
        }

        /// <summary>
        /// Hotspot state enum matching Android WifiManager.WIFI_AP_STATE constants.
        /// </summary>
        public enum HotspotState
        {
            Disabling = 10,
            Disabled = 11,
            Enabling = 12,
            Enabled = 13,
            Failed = 14,
            Unknown = -1
        }
#endif
    }
}
