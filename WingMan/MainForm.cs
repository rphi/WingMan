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
                var dialog = new PleaseWait("Checking for Arduino on " + arduinoSerialPortCombo.Text);
                var dialogthread = new Thread(() => dialog.ShowDialog());
                dialogthread.Start();
                try
                {
                    port.Open();
                }
                catch
                {
                    if (dialogthread.IsAlive) { dialogthread.Abort(); }
                    MessageBox.Show("Unable to open port " + arduinoSerialPortCombo.Text, "Communication error",
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
                    if (dialogthread.IsAlive) { dialogthread.Abort(); }
                    arduinoSerialPortCombo.Enabled = true;
                    MessageBox.Show(
                        "The device on " + arduinoSerialPortCombo.Text +
                        " has not been recognised. Please check it is running WingMan firmware.",
                        "Error communicating with Arduino", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
                arduinoFWverBox.Text = fw;
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
                }
                if (configLibrary.ArduinoConfigs.ContainsKey(arduinoIdBox.Text))
                {
                    currentArduinoConfig = configLibrary.ArduinoConfigs[arduinoIdBox.Text];
                    arduinoConfiguredBox.Checked = true;
                }
                else
                {
                    currentArduinoConfig = new ArduinoConfigObject(arduinoIdBox.Text, arduinoHwInfoBox.Text, int.Parse(arduinoFadersCountBox.Text), int.Parse(arduinoButtonsCountBox.Text));
                    arduinoConfiguredBox.Checked = false;
                }
                arduinoSerialPortCombo.Enabled = true;
                arduinoConfigureButton.Enabled = true;
                if (dialogthread.IsAlive) { dialogthread.Abort();}
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
                MessageBox.Show("Error starting","The source has not been selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IPAddress.TryParse(targetTextBox.Text, out target))
            {
                MessageBox.Show("Error starting","Invalid target IP address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Int32.TryParse(targetPortTextBox.Text, out port))
            {
                MessageBox.Show("Error starting", "Invalid target port", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Error starting", "The source has not been selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            if (connectionGood)
            {
                statusLabel.Text = "Connected";
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
                MessageBox.Show("Error starting Arduino", "Arduino input mapping not configured", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            try
            {
                var args = new ArduinoSourceFactoryArgs(new SerialPort(arduinoSerialPortCombo.Text, 115200), currentArduinoConfig.Faders, currentArduinoConfig.Buttons);

                loop = new Runloop(SourceFactory.CreateSource(SourceMode.Arduino, args),
                    new OscConnection(IPAddress.Parse(targetTextBox.Text), Int32.Parse(targetPortTextBox.Text)), new OscProcessor(currentArduinoConfig.FaderMap, currentArduinoConfig.ButtonMap));
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
            Stop();
            disconnectButton.Enabled = false;
            connectButton.Enabled = true;
            sourceModeCombo.Enabled = true;
            arduinoSerialPortCombo.Enabled = true;
            targetPortTextBox.Enabled = true;
            targetTextBox.Enabled = true;
            statusLabel.Text = "Disconnected";
            statusLabel.BackColor = Color.OrangeRed;
        }

        public void Stop()
        {
            loop.running = false;
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

    public class Runloop
    {
        public bool running = true;
        private ISource source;
        private OscConnection connection;
        private OscProcessor processor;

        public Runloop(ISource s, OscConnection c, OscProcessor p)
        {
            source = s;
            source.NewInputsReady += SendMessages;
            source.NoChange += Loop;
            connection = c;
            processor = p;
        }

        public void Run()
        {
            if (running)
            {
                source.Read();
            }
            else
            {
                Stop();
            }
        }

        public void Loop(object o, EventArgs e)
        {
            Run();
        }

        public void SendMessages(object ins, EventArgs e)
        {
            var input = (List<Input>) ins;
            OscProcessor.SendCommands(processor.MakeCommands(input), connection);
            Run();//loop
        }

        public void Stop()
        {
            source.Close();
            connection.Dispose();
        }
    }
}
