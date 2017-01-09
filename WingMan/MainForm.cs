using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WingMan.Objects;
using WingMan.Osc;

namespace WingMan
{
    public partial class MainForm : Form
    {
        private Task runloopTask;
        private Runloop loop;

        public MainForm()
        {
            InitializeComponent();
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
            if (((SourceMode)sourceModeCombo.SelectedItem == SourceMode.None ||
                ((SourceMode)sourceModeCombo.SelectedItem == SourceMode.Arduino &&
                 arduinoSerialPortCombo.SelectedItem is SerialPort)))
            {
                MessageBox.Show("Error starting","The source has not been configured properly", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try
            {
                loop = new Runloop(SourceFactory.CreateSource(SourceMode.Arduino, new SerialPort(arduinoSerialPortCombo.Text, 115200)),
                    new OscConnection(IPAddress.Parse(targetTextBox.Text), Int32.Parse(targetPortTextBox.Text)));
                runloopTask = new Task(() => loop.Run());
                runloopTask.Start();
            }
            catch
            {
                connectionGood = false;
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
            runloopTask.Dispose();
        }
    }

    public class Runloop : IDisposable
    {
        public bool running = true;
        private ISource source;
        private OscConnection connection;
        private OscProcessor processor = new OscProcessor();

        public Runloop(ISource s, OscConnection c)
        {
            source = s;
            source.NewInputsReady += SendMessages;
            source.NoChange += Loop;
            connection = c;
        }

        public void Run()
        {
            if (running)
            {
                source.Read();
            }
        }

        public void Loop(object o, EventArgs e)
        {
            Run();
        }

        public void SendMessages(object ins, EventArgs e)
        {
            var input = (List<Input>) ins;
            processor.SendCommands(processor.MakeCommands(input), connection);
            Run();//loop
        }

        public void Dispose()
        {
            running = false;
            source.Close();
            connection.Dispose();
        }
    }
}
