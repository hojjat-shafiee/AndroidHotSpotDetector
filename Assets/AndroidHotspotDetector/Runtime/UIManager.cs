using TMPro;
using UnityEngine;
using System.Collections;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace AndroidHotspot
{
    /// <summary>
    /// Simple UI manager for the demo scene.
    /// Handles permission requests and displays hotspot status.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;
        private bool _isInitialized = false;

        void Start()
        {
            if (statusText == null)
            {
                Debug.LogError("[AndroidHotspot] Status Text UI element is not assigned!");
                return;
            }

#if UNITY_ANDROID
            StartCoroutine(InitializePermissions());
#else
        statusText.text = "Hotspot detection is only supported on Android.";
        _isInitialized = true;
#endif
        }

#if UNITY_ANDROID
        private IEnumerator InitializePermissions()
        {
            // ACCESS_WIFI_STATE and ACCESS_NETWORK_STATE are Normal permissions (auto-granted).
            // However, we check them to ensure the environment is ready.
            bool hasWifi = Permission.HasUserAuthorizedPermission("android.permission.ACCESS_WIFI_STATE");
            bool hasNetwork = Permission.HasUserAuthorizedPermission("android.permission.ACCESS_NETWORK_STATE");

            if (!hasWifi || !hasNetwork)
            {
                statusText.text = "Requesting permissions...";
                // In Android 6.0+, these are typically granted at install, but we request them to be safe.
                if (!hasWifi) Permission.RequestUserPermission("android.permission.ACCESS_WIFI_STATE");
                if (!hasNetwork) Permission.RequestUserPermission("android.permission.ACCESS_NETWORK_STATE");
                
                yield return new WaitForSeconds(1f);
            }

            _isInitialized = true;
            statusText.text = "Ready. Tap 'Check Hotspot' to scan.";
#endif
        }

        public void CheckHotspot()
        {
            if (!_isInitialized) return;

            bool isHotspotActive = AndroidHotspotDetector.IsDeviceHotspotEnabled();

            if (isHotspotActive)
            {
                statusText.text = "Status: HOTSPOT ENABLED";
                Debug.Log("[AndroidHotspot] Device is providing hotspot.");
            }
            else
            {
                statusText.text = "Status: HOTSPOT DISABLED";
                Debug.Log("[AndroidHotspot] Device is NOT providing hotspot.");
            }
        }
    }
}
