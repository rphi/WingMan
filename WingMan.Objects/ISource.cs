using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WingMan.Objects
{
    public interface ISource
    {
        void Read();
        bool Send(string s);
        void Close();
        event EventHandler BufferUpdated;
    }

    public enum SourceMode
    {
        None, Arduino
    }

    public class ArduinoSourceFactoryArgs
    {
        public SerialPort Port;
        public int Faders;
        public int Buttons;

        public ArduinoSourceFactoryArgs(SerialPort p, int faders, int buttons)
        {
            Port = p;
            Faders = faders;
            Buttons = buttons;
        }
    }

    public class SourceEventArgs : EventArgs
    {
        public SourceEvent Event;

        public SourceEventArgs(SourceEvent e)
        {
            Event = e;
        }
    }

    public enum SourceEvent
    {
        NoChange, NewItems, IoError
    }
}
