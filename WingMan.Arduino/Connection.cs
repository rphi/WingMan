using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using WingMan.Objects;

namespace WingMan.Arduino
{
    public class Connection : IDisposable
    {
        public SerialPort Port;
        public int Faders;
        public int ButtonBytes;
        public int WordLen;
        private DateTime lastRequest;
        private readonly TimeSpan timeout = TimeSpan.FromMilliseconds(500);

        public Connection(SerialPort port, int faders = 6, int buttons = 8)
        {
            Port = port;
            Port.DataReceived += DataReceived;
            Faders = faders;
            ButtonBytes = (int)Math.Ceiling(buttons/8.0);
            WordLen = ButtonBytes + Faders;
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
            Request();
            lastRequest = DateTime.UtcNow;
        }

        public void DataReceived(object o, EventArgs e)
        {
            if (Port.BytesToRead >= WordLen)
            {
                Byte[] buffer = new byte[WordLen];
                Port.Read(buffer, 0, WordLen);

                var x = 0;
                var i = new List<Input>();
                foreach (var b in buffer)
                {
                    if (x <= Faders - 1)
                    {
                        // current byte is for a fader
                        i.Add(new Input(InputType.Fader, x, (int)Math.Round(b * 0.392)));
                        x = x + 1;
                    }
                    else
                    {
                        // current byte is for a button
                        var id = 0;
                        foreach (bool bit in new BitArray(new byte[]{b}))
                        {
                            id = id + 1;
                            i.Add(new Input(InputType.Button, id, bit ? 0 : 1)); // no, that ternary is the right way around, it's active low
                        }
                    }
                }

                if (i.Count > 0)
                {
                    NewInputsReceived(i, EventArgs.Empty);
                }
            }
            else if (DateTime.UtcNow - lastRequest > timeout) // in case a byte goes missing or something silly?
            {
                Port.DiscardInBuffer();
                Read();
            }
        }

        public event EventHandler NewInputsReceived;

        public void Request()
        {
            Port.Write(new byte[]{0b01010101}, 0, 1); // ascii 'U'
        }
    }
}
