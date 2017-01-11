using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WingMan.Objects;
using WingMan.Osc;

namespace WingMan
{
    public class Runloop
    {
        public bool running = true;
        private ISource source;
        private OscConnection connection;
        private OscProcessor processor;
        //private Stopwatch sw;

        public Runloop(ISource s, OscConnection c, OscProcessor p)
        {
            source = s;
            source.BufferUpdated += BufferUpdated;
            connection = c;
            processor = p;
        }

        public void Run()
        {
            #region debug timer
            /*
            if (sw != null && sw.IsRunning)
            {
                sw.Stop();
                MessageBox.Show("Took " + sw.ElapsedMilliseconds + " for one loop");
            }
            else
            {
                sw = Stopwatch.StartNew();
            }
            */
            #endregion

            if (running)
            {
                source.Read();
            }
            else
            {
                Stop();
            }
        }

        public event EventHandler Stopped;

        private void BufferUpdated(object o, EventArgs args)
        {
            switch (((SourceEventArgs) args).Event)
            {
                case SourceEvent.NoChange:
                    Run();
                    break;
                case SourceEvent.IoError:
                    IoErrorHandler();
                    break;
                case SourceEvent.NewItems:
                    SendMessages((List<Input>) o);
                    break;
            }
        }

        private void SendMessages(List<Input> ins)
        {
            var commands = processor.MakeCommands(ins);
            if (commands.Count != 0)
            {
                OscProcessor.SendCommands(commands, connection);
                new Thread(() => CommandsSent(commands.Select(i => i.ToString()).ToList(), EventArgs.Empty)).Start();
            }
            Run();//loop
        }

        private void Stop()
        {
            source.Close();
            connection.Dispose();
            Stopped("", new RunLoopStoppedEventArgs(RunLoopStopType.Graceful));
        }

        private void IoErrorHandler()
        {
            switch (
                MessageBox.Show("IO timeout communicating with Arduino. Please check the connection and retry.",
                    "Arduino IO Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error))
            {
                case DialogResult.Cancel:
                    Stopped("IO Failure", new RunLoopStoppedEventArgs(RunLoopStopType.Error));
                    break;
                case DialogResult.Retry:
                    Run();
                    break;
            }
        }

        public event EventHandler CommandsSent;
    }

    public class RunLoopStoppedEventArgs : EventArgs
    {
        public RunLoopStopType Type;

        public RunLoopStoppedEventArgs(RunLoopStopType t)
        {
            Type = t;
        }
    }

    public enum RunLoopStopType
    {
        Graceful, Error
    }
}
