using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using WingMan.Objects;

namespace WingMan.Arduino
{
    public class Connection : IDisposable
    {
        public SerialPort Port;
        public int Faders;
        public int ButtonBytes;
        public int WordLen;
        private System.Timers.Timer timer;
        private readonly TimeSpan timeout = TimeSpan.FromMilliseconds(750); // give the Arduino 750ms to respond

        public Connection(SerialPort port, int faders = 6, int buttons = 8)
        {
            Port = port;
            Port.DataReceived += DataReceived;
            Faders = faders;
            ButtonBytes = (int) Math.Ceiling(buttons / 8.0);
            WordLen = ButtonBytes + Faders;
            timer = new System.Timers.Timer(timeout.TotalMilliseconds);
            //timer.Elapsed += OnTimeout;
        }

        public bool Start()
        {
            try
            {
                Port.Open();
            }
            catch
            {
                throw new IOException("Unable to open port");
            }
            return Port.IsOpen;
        }

        public void Dispose()
        {
            if (timer.Enabled)
            {
                timer.Stop();
            }
            if (Port.IsOpen)
            {
                Port.Close();
            }
            Port.Dispose();
        }

        public void Read()
        {
            if (!Port.IsOpen)
            {
                this.Start();
            }
            timer.Start();
            Request();
        }

        public void DataReceived(object o, EventArgs e)
        {
            if (Port.BytesToRead == WordLen)
            {
                timer.Stop();
                Byte[] buffer = new byte[WordLen];
                Port.Read(buffer, 0, WordLen);

                var faderId = 1;
                var buttonId = 1;
                var i = new List<Input>();
                foreach (var b in buffer)
                {
                    if (faderId <= Faders)
                    {
                        // current byte is for a fader
                        i.Add(new Input(InputType.Fader, faderId, (int) Math.Round(b * 0.392)));
                        faderId++;
                    }
                    else
                    {
                        // current byte is for a button
                        foreach (bool bit in new BitArray(new byte[] {b}))
                        {
                            i.Add(new Input(InputType.Button, buttonId, bit ? 0 : 1));
                                // no, that ternary is the right way around, it's active low
                            buttonId++;
                        }
                    }
                }
                if (i.Count > 0)
                {
                    NewInputsReceived(i, EventArgs.Empty);
                }
            }
        }

        public event EventHandler NewInputsReceived;
        public event ElapsedEventHandler IOTimeout;

        public void Request()
        {
            Port.Write(new byte[] {0b01010101}, 0, 1); // ascii 'U'
        }

        public void Reset()
        {
            Port.DiscardInBuffer();
            Read();
        }

        public void OnTimeout(object o, ElapsedEventArgs a)
        {
            timer.Stop();
            if (Port.IsOpen)
            {
                IOTimeout(o, a);
            }
        }
    }
}
