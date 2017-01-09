using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WingMan.Objects;

namespace WingMan.Arduino
{
    public class Connection : IDisposable
    {
        public SerialPort Port { get; set; }
        public int Faders { get; set; }
        public int ButtonBytes { get; set; }
        public int WordLen { get; set; }

        public Connection(SerialPort port, int faders = 6, int buttonbytes = 1)
        {
            Port = port;
            Port.DataReceived += DataReceived;
            Faders = faders;
            ButtonBytes = buttonbytes;
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
        }

        public void DataReceived(object o, EventArgs e)
        {
            if (Port.BytesToRead >= WordLen)
            {
                Byte[] buffer = new byte[WordLen];
                Port.Read(buffer, 0, WordLen);

                var x = 1;
                var i = new List<Input>();
                foreach (var b in buffer)
                {
                    if (x <= Faders)
                    {
                        // current byte is for a fader
                        i.Add(new Input(InputType.Fader, x, b));
                        x = x + 1;
                    }
                    else
                    {
                        // current byte is for a button
                        var id = 0;
                        foreach (bool bit in new BitArray(new byte[]{b}))
                        {
                            id = id + 1;
                            if (bit)
                            {
                                //i.Add(new Input(InputType.Button, (id), 1));
                            }

                        }
                        x = id + x;
                    }
                }
                if (i != null)
                {
                    NewInputsReceived(i, EventArgs.Empty);
                }
            }
        }

        public event EventHandler NewInputsReceived;

        public void Request()
        {
            Port.Write(new byte[]{0b01010101}, 0, 1); // ascii 'U'
        }
    }
}
