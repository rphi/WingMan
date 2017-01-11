using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using WingMan.Objects;
using WingMan.Osc;
using WingMan.Sources;

namespace WingMan
{
    public partial class MainForm : Form
    {
        private Thread runloopTask;
        private Runloop loop;
        private ArduinoConfigObject currentArduinoConfig;
        private ConfigLibrary configLibrary;

        public MainForm()
        {
            InitializeComponent();
            configLibrary = ConfigLibrary.CreateConfigLibrary();
            sourceModeCombo.DataSource = Enum.GetValues(typeof(SourceMode));
        }

        private void sourceLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectTab(sourceTabPage);
        }

        private void sourceModeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (sourceModeCombo.SelectedItem as SourceMode?)
            {
                case SourceMode.None:
                    arduinoSettingsPanel.Visible = false;
                    break;
                case SourceMode.Arduino:
                    sourceTextBox.Text = "Arduino";
                    SetupArduinoMenu();
                    break;
            }
        }

        private void arduinoSerialPortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (arduinoSerialPortCombo.Text != "None found")
            {
                arduinoSerialPortCombo.Enabled = false;
                arduinoConfigureButton.Enabled = false;
                var port = new SerialPort(arduinoSerialPortCombo.Text, 115200);
                var waitdialog = new PleaseWait("Checking for Arduino on " + arduinoSerialPortCombo.Text);
                new Thread(() => waitdialog.ShowDialog()).Start();
                try
                {
                    port.Open();
                }
                catch
                {
                    this.Invoke((Action)delegate { waitdialog.Close(); });
                    MessageBox.Show("Communication error", "Unable to open port " + arduinoSerialPortCombo.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    arduinoFWverBox.Text = "";
                    return;
                }
                port.Write("?");
                Thread.Sleep(1000);
                var fw = port.ReadExisting();
                port.Close();
                if (fw.Split(' ').FirstOrDefault() != "WINGMAN")
                {
                    this.Invoke((Action)delegate { waitdialog.Close(); });
                    arduinoSerialPortCombo.Enabled = true;
                    MessageBox.Show(
                        "Error communicating with Arduino", "The device on " + arduinoSerialPortCombo.Text +
                        " has not been recognised. Please check it is running WingMan firmware.", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                arduinoFWverBox.Text = fw;
                var arduinoVersionNo = "";
                var atoms = fw.Split(' ').ToArray();
                foreach (var i in atoms)
                {
                    var s = i.Split(':');
                    if (s.Length == 1)
                    {
                        // this is just the name
                        continue;
                    }
                    if (s.Length != 2) throw new Exception("Error parsing Arduino firmware data");

                    if (s[0] == "F") { arduinoFadersCountBox.Text = s[1];} // number of faders
                    if (s[0] == "B") { arduinoButtonsCountBox.Text = s[1];} // number of buttons
                    if (s[0] == "P") { arduinoHwInfoBox.Text = s[1];} // hardware platform
                    if (s[0] == "I") { arduinoIdBox.Text = s[1];} // device ID
                    if (s[0] == "V") { arduinoVersionNo = s[1];} // version number
                }
                if (configLibrary.ArduinoConfigs.ContainsKey(arduinoIdBox.Text))
                {
                    var libraryArduinoConfig = configLibrary.ArduinoConfigs[arduinoIdBox.Text];
                    if (libraryArduinoConfig.Buttons == int.Parse(arduinoButtonsCountBox.Text) &&
                        libraryArduinoConfig.Faders == int.Parse(arduinoFadersCountBox.Text) &&
                        libraryArduinoConfig.Version == arduinoVersionNo)
                    {
                        currentArduinoConfig = libraryArduinoConfig;
                        arduinoConfiguredBox.Checked = true;
                    }
                    else
                    {
                        switch (
                            MessageBox.Show(
                                "Another Arduino with the same ID has been found in the configuration library but with different parameters. Do you want to overwrite the existing entry? (you will lose and existing mappings for this device ID)",
                                "Duplicate ID found in library", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                        {
                            case DialogResult.Cancel:
                                MessageBox.Show(
                                    "Please ensure every Arduino has a unique ID. The software will now quit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                                break;
                            case DialogResult.OK:
                                currentArduinoConfig = new ArduinoConfigObject(arduinoIdBox.Text, arduinoHwInfoBox.Text, int.Parse(arduinoFadersCountBox.Text), int.Parse(arduinoButtonsCountBox.Text), arduinoVersionNo);
                                arduinoConfiguredBox.Checked = false;
                                break;
                        }
                    }
                    
                }
                else
                {
                    currentArduinoConfig = new ArduinoConfigObject(arduinoIdBox.Text, arduinoHwInfoBox.Text, int.Parse(arduinoFadersCountBox.Text), int.Parse(arduinoButtonsCountBox.Text), arduinoVersionNo);
                    arduinoConfiguredBox.Checked = false;
                }
                arduinoSerialPortCombo.Enabled = true;
                arduinoConfigureButton.Enabled = true;
                this.Invoke((Action)delegate { waitdialog.Close(); });
            }
            else
            {
                arduinoConfigureButton.Enabled = false;
                arduinoFWverBox.Text = "";
                arduinoButtonsCountBox.Text = "";
                arduinoFadersCountBox.Text = "";
            }
        }

        private void SetupArduinoMenu()
        {
            var ports = SerialPort.GetPortNames();
            arduinoSerialPortCombo.DataSource = ports.Length != 0 ? ports : new []{"None found"};
            arduinoSettingsPanel.Visible = true;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            IPAddress target;
            int port;
            // Check Form
            if ((SourceMode)sourceModeCombo.SelectedItem == SourceMode.None || sourceModeCombo.SelectedItem == null)
            {
                MessageBox.Show("The source has not been selected", "Error starting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IPAddress.TryParse(targetTextBox.Text, out target))
            {
                MessageBox.Show("Invalid target IP address", "Error starting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Int32.TryParse(targetPortTextBox.Text, out port))
            {
                MessageBox.Show("Invalid target port", "Error starting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            connectButton.Enabled = false;
            sourceModeCombo.Enabled = false;
            arduinoSerialPortCombo.Enabled = false;
            targetPortTextBox.Enabled = false;
            targetTextBox.Enabled = false;

            statusLabel.Text = "Connecting...";
            statusLabel.BackColor = Color.Cyan;
            var connectionGood = true;

            switch ((SourceMode) sourceModeCombo.SelectedItem)
            {
                case SourceMode.Arduino:
                    connectionGood = StartArduino();
                    break;
                default:
                    MessageBox.Show("The source has not been selected", "Error starting", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            if (connectionGood)
            {
                statusLabel.Text = "Running";
                statusLabel.BackColor = Color.Green;
                disconnectButton.Enabled = true;
            }
            else
            {
                MessageBox.Show("There was an error starting the software :(", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                connectButton.Enabled = true;
                sourceModeCombo.Enabled = true;
                arduinoSerialPortCombo.Enabled = true;
                targetPortTextBox.Enabled = true;
                targetTextBox.Enabled = true;
                statusLabel.Text = "Error starting";
                statusLabel.BackColor = Color.Red;
            }
        }

        private bool StartArduino()
        {
            if (!arduinoConfiguredBox.Checked)
            {
                MessageBox.Show("Arduino input mapping not configured", "Error starting Arduino", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            try
            {
                var args = new ArduinoSourceFactoryArgs(new SerialPort(arduinoSerialPortCombo.Text, 115200), currentArduinoConfig.Faders, currentArduinoConfig.Buttons);

                loop = new Runloop(SourceFactory.CreateSource(SourceMode.Arduino, args),
                    new OscConnection(IPAddress.Parse(targetTextBox.Text), Int32.Parse(targetPortTextBox.Text)), new OscProcessor(currentArduinoConfig.FaderMap, currentArduinoConfig.ButtonMap));
                loop.Stopped += OnLoopStopped;
                loop.CommandsSent += OnCommandsSent;
                runloopTask = new Thread(loop.Run);
                runloopTask.Start();
            }
            catch 
            {
                return false;
            }
            return true;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Stopping";
            statusLabel.BackColor = Color.Orange;
            loop.running = false;
        }

        private void OnLoopStopped(object o, EventArgs args)
        {
            switch (((RunLoopStoppedEventArgs) args).Type)
            {
                case RunLoopStopType.Graceful:
                    this.Invoke((MethodInvoker) delegate
                    {
                        statusLabel.Text = "Stopped";
                        statusLabel.BackColor = Color.OrangeRed;
                    });
                    break;
                case RunLoopStopType.Error:
                    this.Invoke((MethodInvoker)delegate
                    {
                        statusLabel.Text = (string)o;
                        statusLabel.BackColor = Color.DarkOrchid;
                    });
                    break;
            }
            this.Invoke((MethodInvoker)delegate
            {
                disconnectButton.Enabled = false;
                connectButton.Enabled = true;
                sourceModeCombo.Enabled = true;
                arduinoSerialPortCombo.Enabled = true;
                targetPortTextBox.Enabled = true;
                targetTextBox.Enabled = true;
            });
        }

        private void OnCommandsSent(object o, EventArgs args)
        {
            var commands = (List<String>) o;
            //commands.Add("[" + DateTime.UtcNow.ToString("HH:mm:ss.fff") + "]");
            commands.Reverse();
            var commandsstring = string.Join("; ", commands);
            Invoke((MethodInvoker) delegate
            {
                oscLog.Text = "[" + DateTime.UtcNow.ToString("HH:mm:ss.fff") + "] " + commandsstring + Environment.NewLine + new string(oscLog.Text.Take(1000).ToArray());
            });
        }

        private void arduinoConfigureButton_Click(object sender, EventArgs e)
        {
            using (var form = new ArduinoConfigForm(currentArduinoConfig))
            {
                switch (form.ShowDialog())
                {
                    case DialogResult.OK:
                        arduinoConfiguredBox.Checked = true;
                        configLibrary.AddArduinoConfigObject(form.ArduinoConfig);
                        currentArduinoConfig = form.ArduinoConfig;
                        break;
                    case DialogResult.Cancel:
                        break;
                }
            }
        }
    }
}
