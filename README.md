# TheWootMouse

A C# application that enables analog mouse cursor control using Wooting keyboard analog inputs. This project leverages
the pressure-sensitive nature of Wooting keyboards to provide smooth, proportional cursor movement based on key press
depth.

## Features

- **Analog Cursor Control**: Move your mouse cursor using keyboard keys with variable speed based on how hard you press
- **Configurable Response Curve**: Adjustable deadzone, maximum speed, and exponential response for personalized control
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

| Key | Direction |
|-----|-----------|
| F13 | Up        |
| F14 | Right     |
| F15 | Down      |
| F16 | Left      |

**Note**: These bindings are currently hardcoded but will be user-configurable in future releases.

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

Current configuration parameters can be adjusted in `Program.cs` when creating the `WootMouseEngine`:

```csharp
var engine = new WootMouseEngine(
    deadzone: 0.05f,    // Minimum press threshold (0-1)
    maxSpeed: 1400f,    // Maximum pixels per second
    exponent: 1.6f      // Response curve exponent
);
