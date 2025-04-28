using System;
using System.Windows.Forms;

namespace VolvoECUInterface.App
{
    public partial class UserInterface : Form
    {
        private RP1210Interface _rp1210Interface;
        private bool _isConnected = false;

        public UserInterface()
        {
            InitializeComponent();
            InitializeUI();
        }

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

        private async void ConnectButton_Click(object sender, EventArgs e)
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
                    GetControl<Button>(sender as Button).Text = "Disconnect";
                }
                else
                {
                    _rp1210Interface.Disconnect();
                    _isConnected = false;

                    UpdateStatus("Status: Disconnected");
                    GetControl<Button>("identifyButton").Enabled = false;
                    GetControl<Button>(sender as Button).Text = "Connect to Adapter";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void IdentifyButton_Click(object sender, EventArgs e)
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

        private void UpdateStatus(string status)
        {
            GetControl<Label>("statusLabel").Text = status;
        }

        private T GetControl<T>(string name) where T : Control
        {
            var control = this.Controls.Find(name, true)[0];
            return (T)control.Tag;
        }

        private T GetControl<T>(Control control) where T : Control
        {
            return (T)control;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_rp1210Interface != null)
            {
                _rp1210Interface.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
