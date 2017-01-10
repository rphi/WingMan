using System;
using System.Collections.Generic;
using System.Linq;
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
                outputBuffer = newInputs;
            }
            if (newInputs != null)
            {
                if (lastInputs == null)
                {
                    outputBuffer = newInputs;
                    lastInputs = newInputs;
                    NewInputsReady(outputBuffer, EventArgs.Empty);
                }
                else
                {
                    var changedInputs = newInputs.Except(lastInputs, new InputComparer()).ToList();
                    lastInputs = newInputs;
                    if (changedInputs.Count != 0)
                    {
                        outputBuffer = changedInputs.ToList();
                        NewInputsReady(outputBuffer, EventArgs.Empty);
                    }
                    else
                    {
                        NoChange(new object(), EventArgs.Empty);
                    }
                }
            }
            else
            {
                NoChange(new object(), EventArgs.Empty);
            }
        }

        public event EventHandler NewInputsReady;

        public event EventHandler NoChange;

        public bool Send(string s)
        {
            return true;
        }

        public void Close()
        {
            _connection.Dispose();
        }
    }
}
