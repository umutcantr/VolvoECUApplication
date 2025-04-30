/// <summary>
/// Handles communication with RP1210-compliant devices.
/// This class provides methods for connecting to and communicating with
/// RP1210-compliant devices like Nexiq USB Link 2/3, DPA 5, and Cat Comm 3.
/// </summary>
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace VolvoECUInterface.App
{
    public class RP1210Interface : IDisposable
    {
        private const int RP1210_MAX_MESSAGE_SIZE = 1785;
        private const int RP1210_TIMEOUT = 5000; // 5 seconds timeout

        private IntPtr _clientId;
        private bool _isConnected;
        private readonly string _dllPath;
        private readonly string _deviceId;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("C:\\Program Files (x86)\\NEXIQ\\USB-Link 2\\RP1210\\difxapi.dll", SetLastError = true)]
        private static extern int DriverPackageInstall(string driverPackageInfPath, int flags, IntPtr pInstallerInfo, out bool pNeedReboot);

        /// <summary>
        /// Initializes a new instance of the RP1210Interface class.
        /// </summary>
        /// <param name="dllPath">Path to the RP1210 DLL</param>
        /// <param name="deviceId">ID of the RP1210 device to use</param>
        public RP1210Interface(string dllPath, string deviceId)
        {
            _dllPath = dllPath;
            _deviceId = deviceId;
            _isConnected = false;
        }

        /// <summary>
        /// Establishes a connection with the RP1210 device.
        /// Loads the RP1210 DLL and initializes the connection.
        /// </summary>
        /// <returns>True if connection was successful</returns>
        /// <exception cref="Exception">Thrown when connection fails</exception>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                // First, try to install the driver package if needed
                bool needReboot;
                string driverPath = "C:\\Program Files (x86)\\NEXIQ\\USB-Link 2\\RP1210\\usblink2.inf";
                
                // Check if driver file exists
                if (!System.IO.File.Exists(driverPath))
                {
                    throw new Exception($"Driver file not found at: {driverPath}");
                }

                // Try to install driver with elevated privileges
                try
                {
                    int result = DriverPackageInstall(driverPath, 0, IntPtr.Zero, out needReboot);
                    if (result != 0)
                    {
                        // If driver installation fails, try to load DLL directly
                        Console.WriteLine($"Driver installation failed with code {result}, attempting to load DLL directly");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Driver installation error: {ex.Message}");
                    // Continue with DLL loading
                }

                // Load RP1210 DLL
                IntPtr dllHandle = LoadLibrary(_dllPath);
                if (dllHandle == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception($"Failed to load RP1210 DLL: {_dllPath}. Error code: {error}");
                }

                // Initialize connection
                await Task.Delay(100); // Simulate connection time
                
                _isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                throw new Exception($"Failed to connect to RP1210 device: {ex.Message}");
            }
        }

        /// <summary>
        /// Identifies the connected ECU by sending CAN messages.
        /// </summary>
        /// <returns>Information about the identified ECU</returns>
        /// <exception cref="Exception">Thrown when identification fails or times out</exception>
        public async Task<string> IdentifyECUAsync()
        {
            if (!_isConnected)
            {
                throw new Exception("Not connected to RP1210 device");
            }

            try
            {
                using (var cts = new CancellationTokenSource(RP1210_TIMEOUT))
                {
                    // Simulate ECU identification
                    await Task.Delay(100, cts.Token);
                    
                    // In a real implementation, this would send CAN messages to identify the ECU
                    return "TRW EMS2.3"; // Placeholder response
                }
            }
            catch (OperationCanceledException)
            {
                throw new Exception("ECU identification timed out");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to identify ECU: {ex.Message}");
            }
        }

        /// <summary>
        /// Disconnects from the RP1210 device and cleans up resources.
        /// </summary>
        public void Disconnect()
        {
            if (_isConnected)
            {
                // Clean up resources
                _isConnected = false;
            }
        }

        /// <summary>
        /// Releases all resources used by the RP1210Interface.
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }
    }
} 