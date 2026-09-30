# Android Hotspot Detector for Unity

A lightweight, dependency-free Unity plugin that detects whether an Android device is currently
providing a **Wi-Fi / USB / Bluetooth hotspot (tether)**.

It exposes a tiny static API you can call from anywhere, ships an automatic `AndroidManifest.xml`
with the required permissions, and includes a ready-to-run demo scene.

---

## Features

*   **Reliable dual detection** – combines `ConnectivityManager` tethering state with the
    `WifiManager` AP state so it works across a wide range of devices and OS versions.
*   **Broad Android support** – Android 4.0 (API 14) through Android 15 (API 35).
*   **Cross-platform API** – compiles everywhere; on non-Android platforms and in the Editor the
    calls are safe no-ops that return `false` / `HotspotState.Unknown`.
*   **Zero runtime dependencies** – the core assembly references nothing but Unity. No TextMeshPro,
    no UGUI, no third-party packages required to use the API.
*   **Clean integration** – proper namespace (`AndroidHotspot`), assembly definitions, XML-doc
    comments, and an automatic Android manifest with install-time permissions.
*   **Demo scene included** – a UI you can build to a device and test in seconds.

## Requirements

*   **Unity 2020.3 LTS or newer** (developed and tested on Unity 6 / 6000.x).
*   **Android Build Support** module installed for the Editor.
*   Target platform set to **Android** when building.

> The **demo scene** additionally uses **TextMeshPro** and the **Universal Render Pipeline (URP)**
> and the **Input System** package, because it ships a URP-based UI. The core detection API has none
> of these requirements.

## Installation

1.  Import the package into your project:
    *   **`.unitypackage`** – double-click the file (or `Assets > Import Package > Custom Package…`)
        and import everything under `AndroidHotspotDetector/`.
    *   **Manual** – copy the `AndroidHotspotDetector/` folder into your project's `Assets/` folder.
2.  If you want to run the demo, make sure **TextMeshPro Essentials** are imported
    (`Window > TextMeshPro > Import TMP Essential Resources`) and that your project uses URP.
3.  Set the build platform to **Android** (`File > Build Settings`).

## Quick Start

```csharp
using AndroidHotspot;

void Update()
{
    // Simplest call: is the device sharing its connection right now?
    bool isHotspot = AndroidHotspotDetector.IsDeviceHotspotEnabled();

    if (isHotspot)
    {
        // ... e.g. reduce bandwidth, warn the player, etc.
    }
}
```

That's it. No setup, no scene objects, no permission prompts.

## API Reference

All types live in the `AndroidHotspot` namespace.

### `AndroidHotspotDetector` (static class)

| Method | Returns | Description |
| --- | --- | --- |
| `IsDeviceHotspotEnabled()` | `bool` | `true` if the device is providing any hotspot / tether. The main entry point. |
| `IsAnyTetherActive()` | `bool` | `true` if any tethered interface (Wi-Fi, USB, Bluetooth) is up. |
| `GetHotspotState()` | `HotspotState` | Detailed Wi-Fi AP state, or `HotspotState.Unknown`. |

### `HotspotState` (enum)

Mirrors the Android `WifiManager.WIFI_AP_STATE_*` constants:

| Value | Meaning |
| --- | --- |
| `Unknown` (-1) | State could not be determined (non-Android platform or query failed). |
| `Disabling` (10) | The access point is turning off. |
| `Disabled` (11) | The access point is off. |
| `Enabling` (12) | The access point is turning on. |
| `Enabled` (13) | The access point is on and providing connectivity. |
| `Failed` (14) | The access point failed to start. |

```csharp
var state = AndroidHotspotDetector.GetHotspotState();
if (state == AndroidHotspotDetector.HotspotState.Enabled)
{
    Debug.Log("Hotspot is fully up.");
}
```

## Permissions

The plugin ships `Plugins/Android/AndroidManifest.xml` declaring:

*   `android.permission.ACCESS_WIFI_STATE`
*   `android.permission.ACCESS_NETWORK_STATE`

Both are **"normal"** permissions, granted automatically at install time — **no runtime prompt** and
no manual manifest editing is required. Unity merges this manifest into your built APK/AAB.

## Demo

Open `AndroidHotspotDetector/Demo/Demo.unity` and press Play, or build it to an Android device
(detection only works on real hardware). Tap **Check Hotspot** to update the status label.

*   The demo's `UIManager` is wired to a UI Button's `OnClick()` and a TextMeshPro label.
*   The `Demo` scripts live in their own assembly (`AndroidHotspotDetector.Demo`) so the core stays
    dependency-free.

## Platform Notes & Troubleshooting

*   **Editor returns `false` / `Unknown`.** Expected. Hotspot state can only be read on a real
    Android device; the Editor and other platforms are safe no-ops.
*   **`GetHotspotState()` returns `Unknown` on device.** Some OEM builds restrict the hidden
    `getWifiApState()` API. `IsDeviceHotspotEnabled()` still works via the tethering check, which is
    the more reliable signal on modern Android.
*   **Detection works over USB/Bluetooth tethering too**, not just Wi-Fi hotspots.
*   **URP / TMP errors from the demo.** These only affect `Demo/`. Install TMP Essentials and use
    URP to run the demo, or simply delete the `Demo/` folder if you only need the core API — the
    core has no dependency on either.

## Support

For bug reports and feature requests, please contact the publisher through the Asset Store support
page or open an issue in the project repository. See `CHANGELOG.md` for release history.

## License

Distributed under the MIT License — see `LICENSE.md`. (When publishing to the Asset Store, you may
replace this with the Unity Asset Store EULA or your own terms.)

