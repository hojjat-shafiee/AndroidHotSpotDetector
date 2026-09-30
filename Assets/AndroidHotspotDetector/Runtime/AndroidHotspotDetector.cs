using System;
using UnityEngine;

namespace AndroidHotspot
{
    /// <summary>
    /// Detects whether an Android device is currently providing a hotspot / tether
    /// (Wi-Fi, USB or Bluetooth).
    ///
    /// <para>
    /// Supported on Android 4.0 (API 14) through Android 15 (API 35). The detector combines a
    /// <c>ConnectivityManager</c> tethering query (the most reliable signal on modern Android)
    /// with a <c>WifiManager</c> AP-state query for maximum coverage across OS versions.
    /// </para>
    ///
    /// <para>
    /// The public API is available on every platform. On non-Android platforms and in the Unity
    /// Editor the queries are safe no-ops that return <c>false</c> / <see cref="HotspotState.Unknown"/>,
    /// so the same calling code compiles and runs everywhere.
    /// </para>
    ///
    /// <para>
    /// Requires the following install-time ("normal") permissions, declared automatically by the
    /// plugin in <c>Plugins/Android/AndroidManifest.xml</c> and granted at install:
    /// <c>android.permission.ACCESS_WIFI_STATE</c> and <c>android.permission.ACCESS_NETWORK_STATE</c>.
    /// No runtime permission prompt is required.
    /// </para>
    /// </summary>
    public static class AndroidHotspotDetector
    {
        /// <summary>
        /// Wi-Fi access-point states, matching the Android <c>WifiManager.WIFI_AP_STATE_*</c> constants.
        /// </summary>
        public enum HotspotState
        {
            /// <summary>The state could not be determined (non-Android platform, or the query failed).</summary>
            Unknown = -1,

            /// <summary>The access point is in the process of being disabled.</summary>
            Disabling = 10,

            /// <summary>The access point is off.</summary>
            Disabled = 11,

            /// <summary>The access point is in the process of being enabled.</summary>
            Enabling = 12,

            /// <summary>The access point is on and providing connectivity.</summary>
            Enabled = 13,

            /// <summary>The access point failed to start.</summary>
            Failed = 14
        }

        /// <summary>
        /// Returns <c>true</c> if the device is currently providing a hotspot / tether of any kind.
        /// This is the simplest, most common entry point.
        /// </summary>
        /// <returns>
        /// <c>true</c> when a hotspot is active; otherwise <c>false</c>. Always <c>false</c> on
        /// non-Android platforms and in the Unity Editor.
        /// </returns>
        public static bool IsDeviceHotspotEnabled()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Tethering interfaces are the most reliable signal on modern Android, where the
            // hidden getWifiApState() API may be restricted for non-system apps.
            if (IsAnyTetherActive())
                return true;

            // Fall back to the Wi-Fi AP state for devices/versions where tethering is not exposed.
            return GetHotspotState() == HotspotState.Enabled;
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns <c>true</c> if any tethered interface (Wi-Fi, USB or Bluetooth) is currently active.
        /// Useful when you need to know whether the device shares its connection, independent of the
        /// Wi-Fi AP state.
        /// </summary>
        /// <returns>
        /// <c>true</c> when a tethered interface is up; otherwise <c>false</c>. Always <c>false</c>
        /// on non-Android platforms and in the Unity Editor.
        /// </returns>
        public static bool IsAnyTetherActive()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (AndroidJavaObject connectivity = GetSystemService("connectivity"))
                {
                    if (connectivity == null)
                        return false;

                    // ConnectivityManager.getTetheredIfaces() -> active tether interface names.
                    string[] ifaces = connectivity.Call<string[]>("getTetheredIfaces");
                    return ifaces != null && ifaces.Length > 0;
                }
            }
            catch (Exception e)
            {
                // The tethering API is hidden/system-only on some builds; degrade gracefully.
                Debug.LogWarning($"[AndroidHotspot] Tethering query unavailable: {e.Message}");
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns the detailed Wi-Fi access-point state, or <see cref="HotspotState.Unknown"/> when it
        /// cannot be determined (non-Android platform, restricted hidden API, or query failure).
        /// </summary>
        public static HotspotState GetHotspotState()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (AndroidJavaObject wifi = GetSystemService("wifi"))
                {
                    if (wifi == null)
                        return HotspotState.Unknown;

                    // WifiManager.getWifiApState() returns 10..14 (see HotspotState).
                    int state = wifi.Call<int>("getWifiApState");
                    return Enum.IsDefined(typeof(HotspotState), state)
                        ? (HotspotState)state
                        : HotspotState.Unknown;
                }
            }
            catch (Exception e)
            {
                // getWifiApState() is a hidden API and may be blocked on Android 9+; degrade gracefully.
                Debug.LogWarning($"[AndroidHotspot] Wi-Fi AP query unavailable: {e.Message}");
                return HotspotState.Unknown;
            }
#else
            return HotspotState.Unknown;
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        /// <summary>
        /// Fetches an Android system service by name through the current Unity activity.
        /// The caller owns (and should dispose) the returned object.
        /// </summary>
        private static AndroidJavaObject GetSystemService(string serviceName)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                return activity != null
                    ? activity.Call<AndroidJavaObject>("getSystemService", serviceName)
                    : null;
            }
        }
#endif
    }
}
