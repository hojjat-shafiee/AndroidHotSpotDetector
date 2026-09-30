# Changelog

All notable changes to **Android Hotspot Detector** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-09-30

### Added
- Cross-platform public API: `AndroidHotspotDetector.IsDeviceHotspotEnabled()`,
  `AndroidHotspotDetector.IsAnyTetherActive()` and `AndroidHotspotDetector.GetHotspotState()`.
- `HotspotState` enum mirroring the Android `WifiManager.WIFI_AP_STATE_*` constants.
- Dual detection strategy combining `ConnectivityManager` tethering state and
  `WifiManager` AP state for maximum reliability across OS versions.
- Automatic `Plugins/Android/AndroidManifest.xml` declaring `ACCESS_WIFI_STATE` and
  `ACCESS_NETWORK_STATE` (both granted at install time).
- Assembly definitions that keep the core dependency-free and scope the TextMeshPro
  dependency to the demo assembly only.
- Demo scene (`Demo/Demo.unity`) with a TextMeshPro UI and a "Check Hotspot" button.

### Notes
- Supported platforms: Android 4.0 (API 14) through Android 15 (API 35).
- On non-Android platforms and in the Unity Editor the API is a safe no-op that returns
  `false` / `HotspotState.Unknown`.
