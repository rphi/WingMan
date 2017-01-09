using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Rug.Osc;
using WingMan.Objects;

namespace WingMan.Osc
{
    public class OscProcessor
    {
        public OscMessage BuildMessage(Input input)
        {
            switch (input.Type)
            {
                case InputType.Button:
                    return input.Value == 1 ? new OscMessage("/eos/sub/" + input.Id) : null;
                case InputType.Fader:
                    return new OscMessage("/eos/chan/" + input.Id, Math.Round(input.Value * 0.39));
                default:
                    return null;
            }
        }

        public List<OscMessage> MakeCommands(List<Input> inputs)
        {
            var commands = new List<OscMessage>();
            foreach (var input in inputs)
            {
                var msg = BuildMessage(input);
                commands.Add(msg);
            }
            return commands;
        }

        public void SendCommands(List<OscMessage> messages, OscConnection connection)
        {
            foreach (var message in messages)
            {
                connection.Send(message);
            }
        }
    }

    public class OscConnection : IDisposable
    {
        private OscSender Sender { get; set; }

        public OscConnection(IPAddress ip, int port)
        {
            Sender = new OscSender(ip, port);
            Sender.Connect();
        }

        public void Dispose()
        {
            Sender.Close();
            Sender.Dispose();
        }

        public void Send(OscMessage s)
        {
            if (Sender.State == OscSocketState.NotConnected)
            {
                Sender.Connect();
            }
            Sender.Send(s);
        }
    }
}
