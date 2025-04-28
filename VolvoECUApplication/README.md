# Volvo ECU Interface

A lightweight Windows application for interfacing with TRW EMS2.2 and EMS2.3 ECUs used in Volvo trucks over the CAN bus.

## Features

- Connect to RP1210-compliant devices (Nexiq USB Link 2/3, DPA 5, Cat Comm 3)
- Identify TRW EMS2.2/2.3 ECUs
- Basic error handling and timeout detection
- Simple and intuitive user interface

## Requirements

- Windows 10 or later
- .NET 6.0 or later
- RP1210-compliant device driver installed
- Compatible adapter (Nexiq USB Link 2/3, DPA 5, Cat Comm 3)

## Installation

1. Ensure you have the .NET 6.0 SDK installed
2. Clone this repository
3. Build the solution:
   ```
   dotnet build
   ```
4. Run the application:
   ```
   dotnet run --project VolvoECUInterface.App
   ```

## Usage

1. Connect your RP1210-compliant device to your computer
2. Launch the application
3. Click "Connect to Adapter" to establish connection
4. Once connected, click "Identify ECU" to identify the connected ECU

## Error Handling

The application includes basic error handling for:
- Connection failures
- Timeout detection (5 seconds)
- Invalid device responses
- Communication errors

## Development

This application is built using:
- C# and .NET 6.0
- Windows Forms for the user interface
- RP1210 API for CAN communication

## License

This project is licensed under the MIT License - see the LICENSE file for details. 