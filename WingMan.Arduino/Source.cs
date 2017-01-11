using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using WingMan.Objects;

namespace WingMan.Arduino
{
    public class Source : ISource
    {
        private readonly Connection _connection;
        private List<Input> lastInputs;
        private List<Input> outputBuffer;

        public Source(Objects.ArduinoSourceFactoryArgs args)
        {
            _connection = new Connection(args.Port,args.Faders,args.Buttons);
            _connection.NewInputsReceived += Process;
            _connection.IOTimeout += ConnectionIOTimeout;
        }

        public void Read()
        {
            _connection.Read();
        }

        protected void Process(object i, EventArgs e)
        {
            var newInputs = (List<Input>) i;
            if (lastInputs == null)
            {
                lastInputs = newInputs;
                BufferUpdated(new object(), new SourceEventArgs(SourceEvent.NoChange));
                return;
            }
            if (newInputs != null)
            {
                var changedInputs = new List<Input>();
                foreach (var input in newInputs)
                {
                    var oldinput = lastInputs.FirstOrDefault(x => x.Id == input.Id && x.Type == input.Type);
                    if (oldinput?.Value != input.Value)
                    {
                        changedInputs.Add(input);
                    }
                }
                lastInputs = newInputs;
                if (changedInputs.Count != 0)
                {
                    outputBuffer = changedInputs.ToList();
                    BufferUpdated(outputBuffer, new SourceEventArgs(SourceEvent.NewItems));
                }
                else
                {
                    BufferUpdated(new object(), new SourceEventArgs(SourceEvent.NoChange));
                }
            }
            else
            {
                BufferUpdated(new object(), new SourceEventArgs(SourceEvent.NoChange));
            }
        }

        public event EventHandler BufferUpdated;

        public bool Send(string s)
        {
            return true;
        }

        public void Close()
        {
            _connection.Dispose();
        }

        public void ConnectionIOTimeout(object o, ElapsedEventArgs e)
        {
            BufferUpdated(o, new SourceEventArgs(SourceEvent.IoError));
        }
    }
}
