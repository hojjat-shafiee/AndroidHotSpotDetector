# Android Hotspot Detector for Unity

A lightweight, robust Unity plugin for detecting if an Android device is providing a WiFi hotspot.

## Features

*   **Cross-Version Compatibility:** Works on Android 4.0 (API 14) through Android 15 (API 35).
*   **Dual Detection Strategy:** Uses both `ConnectivityManager` (for tethered interfaces) and `WifiManager` (hidden API) to ensure maximum reliability.
*   **Asset Store Ready:** Proper assembly definitions, namespaces, and clean architecture.
*   **Simple API:** Static methods for easy integration.

## Installation

1.  Download the `.unitypackage` or import the `AndroidHotspotDetector` folder into your Unity project's `Assets` directory.
2.  Ensure your project has **TextMeshPro** imported (the plugin includes an `.asmdef` reference to it).
3.  Add the `Demo` scene from `Assets/AndroidHotspotDetector/Samples~/Demo/` to your project.

## Usage

```csharp
using AndroidHotspot;

// Check if hotspot is enabled
bool isHotspot = AndroidHotspotDetector.IsDeviceHotspotEnabled();

// Get specific state
AndroidHotspotDetector.HotspotState state = AndroidHotspotDetector.GetHotspotState();
```

## Permissions

The plugin automatically includes the following permissions in its `AndroidManifest.xml`:
*   `android.permission.ACCESS_WIFI_STATE`
*   `android.permission.ACCESS_NETWORK_STATE`

These are "Normal" permissions and are granted automatically at install time.

## Requirements

*   Unity 2020.3 LTS or newer.
*   Android Build Support.
*   TextMeshPro package (included by default in most Unity versions).
