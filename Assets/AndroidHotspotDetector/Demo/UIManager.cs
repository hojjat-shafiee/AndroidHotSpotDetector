using TMPro;
using UnityEngine;

namespace AndroidHotspot
{
    /// <summary>
    /// Demo controller for the Android Hotspot Detector sample scene.
    /// Wire a UI Button's OnClick() event to <see cref="CheckHotspot"/> and assign a
    /// TextMeshProUGUI label to <see cref="statusText"/> to display the result.
    ///
    /// <para>
    /// This class is only used by the demo. The core API (<c>AndroidHotspotDetector</c>) has no
    /// dependency on TextMeshPro or UGUI, so your own code can call it without any UI package.
    /// </para>
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;

        private void Start()
        {
            if (statusText == null)
            {
                Debug.LogError("[AndroidHotspot] Assign a TextMeshProUGUI to the Status Text field.", this);
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            statusText.text = "Ready. Tap 'Check Hotspot' to scan.";
#else
            statusText.text = "Hotspot detection runs on Android devices.";
#endif
        }

        /// <summary>
        /// Queries the detector and updates the status label. Hook this up to a UI Button.
        /// </summary>
        public void CheckHotspot()
        {
            if (statusText == null)
                return;

            bool isActive = AndroidHotspotDetector.IsDeviceHotspotEnabled();

            statusText.text = isActive
                ? "Status: HOTSPOT ENABLED"
                : "Status: HOTSPOT DISABLED";

            Debug.Log(isActive
                ? "[AndroidHotspot] Device is providing a hotspot."
                : "[AndroidHotspot] Device is not providing a hotspot.");
        }
    }
}
