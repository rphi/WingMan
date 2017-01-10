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
                lastInputs = newInputs;
                NoChange(new object(), EventArgs.Empty);
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
                    NewInputsReady(outputBuffer, EventArgs.Empty);
                }
                else
                {
                    NoChange(new object(), EventArgs.Empty);
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
