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
        bool Connect();
        void Read();
        bool Send(string s);
        void Close();
        event EventHandler NewInputsReady;
        event EventHandler NoChange;
    }

    public enum SourceMode
    {
        None, Arduino
    }
}
