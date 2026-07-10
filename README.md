# TheWootMouse

A C# application that enables analog mouse cursor control using Wooting keyboard analog inputs. This project leverages
the pressure-sensitive nature of Wooting keyboards to provide smooth, proportional cursor movement based on key press
depth.

## Features

- **Analog Cursor Control**: Move your mouse cursor using keyboard keys with variable speed based on how hard you press
- **Configurable Response Curve**: Adjustable deadzone, maximum speed, and exponential response for personalized control
- **Pixel-Density Profiles**: Keep perceived cursor speed constant across screens of differing pixel density — define a
  profile per screen (real PPI) and have the engine auto-select it based on the monitor the cursor is on
- **Settings App**: A dark-themed WPF UI (`WootMouseSettingsWpf`) for key bindings, mouse behavior, density profiles, and
  a live analog preview
- **Low Latency**: High-frequency polling (~8ms sleep) for responsive cursor movement
- **SDK Integration**: Built on top of the official Wooting Analog SDK

## Requirements

- **Hardware**: Wooting keyboard with analog input support (e.g., Wooting 60HE, Wooting Two HE)
- **Software**:
    - .NET 10.0 or later
    - Windows operating system (uses `user32.dll` for cursor control)
    - `wooting_analog_sdk.dll` (must be placed in the application directory)

## Current Key Bindings

The following keys are currently mapped to mouse movement:

| Key | Direction   |
|-----|-------------|
| F13 | Up          |
| F14 | Right       |
| F15 | Down        |
| F16 | Left        |
| F17 | Scroll Up   |
| F18 | Scroll Down |

**Note**: These are the default bindings. They (and all other settings) are user-configurable through the settings app —
see [Configuration](#configuration).

## Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TheWootMouse
   ```

2. **Install Wooting Analog SDK**
    - Download the `wooting_analog_sdk.dll` from
      the [official Wooting SDK repository](https://github.com/WootingKb/wooting-analog-sdk)
    - Place the DLL in the project's output directory (e.g., `bin/Debug/net10.0/`)

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

## Configuration

Run the **settings app** to configure everything through a UI:

```bash
dotnet run --project WootMouseSettingsWpf
```

It lets you set key bindings, mouse behavior (deadzone, max speed, scroll speed, turbo multiplier, curve power), and
pixel-density profiles, with a live analog preview. Settings are written to `settings.json`, which the engine
(`TheWootMouse`) loads on startup.

### Pixel-density profiles

`Max Speed` is defined in px/s **at the reference PPI** (default 96). Each density profile stores a screen's true
physical PPI; the engine scales speed by `profilePPI / referencePPI` so movement feels the same regardless of how small
the pixels are. Enable **auto-switch** to bind profiles to monitors and have the engine pick the right one based on where
the cursor is — no manual switching when you move between screens. You can enter PPI directly or compute it from a
resolution + diagonal.

> Note: Windows' per-monitor DPI reports the *scaling* setting, not physical pixel density, which is why profiles store
> real PPI explicitly.
