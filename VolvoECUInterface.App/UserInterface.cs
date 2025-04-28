/// <summary>
/// Main user interface form for the Volvo ECU Interface application.
/// This form provides a graphical interface for connecting to RP1210 devices
/// and identifying TRW EMS2.2/2.3 ECUs.
/// </summary>
using System;
using System.Windows.Forms;

namespace VolvoECUInterface.App;

public partial class UserInterface : Form
{
    private RP1210Interface? _rp1210Interface;
    private bool _isConnected = false;

    /// <summary>
    /// Initializes the UserInterface form and sets up the UI components.
    /// </summary>
    public UserInterface()
    {
        InitializeComponent();
        InitializeUI();
    }

    /// <summary>
    /// Sets up the user interface with all necessary controls:
    /// - Connect/Disconnect button
    /// - Identify ECU button
    /// - Status label
    /// - Results text box
    /// </summary>
    private void InitializeUI()
    {
        this.Text = "Volvo ECU Interface";
        this.Size = new System.Drawing.Size(600, 400);

        // Create controls
        var connectButton = new Button
        {
            Text = "Connect to Adapter",
            Location = new System.Drawing.Point(20, 20),
            Size = new System.Drawing.Size(150, 30)
        };
        connectButton.Click += ConnectButton_Click;

        var identifyButton = new Button
        {
            Text = "Identify ECU",
            Location = new System.Drawing.Point(20, 60),
            Size = new System.Drawing.Size(150, 30),
            Enabled = false
        };
        identifyButton.Click += IdentifyButton_Click;

        var statusLabel = new Label
        {
            Text = "Status: Disconnected",
            Location = new System.Drawing.Point(20, 100),
            Size = new System.Drawing.Size(300, 20)
        };

        var resultTextBox = new TextBox
        {
            Multiline = true,
            Location = new System.Drawing.Point(20, 130),
            Size = new System.Drawing.Size(540, 200),
            ReadOnly = true
        };

        // Add controls to form
        this.Controls.Add(connectButton);
        this.Controls.Add(identifyButton);
        this.Controls.Add(statusLabel);
        this.Controls.Add(resultTextBox);

        // Store references to controls we need to access later
        this.Controls.Add(new Control { Name = "statusLabel", Tag = statusLabel });
        this.Controls.Add(new Control { Name = "resultTextBox", Tag = resultTextBox });
        this.Controls.Add(new Control { Name = "identifyButton", Tag = identifyButton });
    }

    /// <summary>
    /// Handles the Connect/Disconnect button click event.
    /// Establishes or terminates connection with the RP1210 device.
    /// </summary>
    /// <param name="sender">The button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void ConnectButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!_isConnected)
            {
                // Initialize RP1210 interface
                _rp1210Interface = new RP1210Interface(
                    "RP1210.dll", // Path to RP1210 DLL
                    "1" // Device ID (example)
                );

                await _rp1210Interface.ConnectAsync();
                _isConnected = true;

                UpdateStatus("Status: Connected");
                GetControl<Button>("identifyButton").Enabled = true;
                if (sender is Button button)
                {
                    button.Text = "Disconnect";
                }
            }
            else
            {
                _rp1210Interface?.Disconnect();
                _isConnected = false;

                UpdateStatus("Status: Disconnected");
                GetControl<Button>("identifyButton").Enabled = false;
                if (sender is Button button)
                {
                    button.Text = "Connect to Adapter";
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Handles the Identify ECU button click event.
    /// Attempts to identify the connected ECU and displays the results.
    /// </summary>
    /// <param name="sender">The button that triggered the event</param>
    /// <param name="e">Event arguments</param>
    private async void IdentifyButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (_isConnected && _rp1210Interface != null)
            {
                string ecuInfo = await _rp1210Interface.IdentifyECUAsync();
                GetControl<TextBox>("resultTextBox").Text = $"ECU Identified: {ecuInfo}";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ECU identification error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Updates the status label with the current connection state.
    /// </summary>
    /// <param name="status">The status message to display</param>
    private void UpdateStatus(string status)
    {
        GetControl<Label>("statusLabel").Text = status;
    }

    /// <summary>
    /// Retrieves a control by its name from the form's control collection.
    /// </summary>
    /// <typeparam name="T">The type of control to retrieve</typeparam>
    /// <param name="name">The name of the control</param>
    /// <returns>The found control cast to the specified type</returns>
    private T GetControl<T>(string name) where T : Control
    {
        var control = this.Controls.Find(name, true)[0];
        return (T)control.Tag;
    }

    /// <summary>
    /// Casts a control to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the control to</typeparam>
    /// <param name="control">The control to cast</param>
    /// <returns>The control cast to the specified type</returns>
    private T GetControl<T>(Control? control) where T : Control
    {
        if (control == null)
        {
            throw new ArgumentNullException(nameof(control));
        }
        return (T)control;
    }

    /// <summary>
    /// Handles the form closing event.
    /// Ensures proper cleanup of RP1210 resources.
    /// </summary>
    /// <param name="e">Form closing event arguments</param>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _rp1210Interface?.Dispose();
        base.OnFormClosing(e);
    }
}
