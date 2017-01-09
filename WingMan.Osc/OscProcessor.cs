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
        private readonly string[] faderCommandMap;
        private readonly OscButtonCommandMap[] buttonCommandMap;

        public OscProcessor(string[] faderMap, OscButtonCommandMap[] buttonMap)
        {
            faderCommandMap = faderMap;
            buttonCommandMap = buttonMap;
        }

        public MaybeOscMessage BuildMessage(Input input)
        {
            switch (input.Type)
            {
                case InputType.Button:
                    return BuildButtonMessage(input);
                case InputType.Fader:
                    return BuildFaderMessage(input);
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
                if (!msg.IsNull)
                {
                    commands.Add(msg.Message);
                }
            }
            return commands;
        }

        private MaybeOscMessage BuildButtonMessage(Input input)
        {
            if (input.Id <= buttonCommandMap.Length)
            {
                var map = buttonCommandMap[input.Id];
                if (map == null)
                {
                    return new MaybeOscMessage();
                }
                switch (map.Type)
                {
                    case OscButtonType.FireOnly:
                        return new MaybeOscMessage(new OscMessage(map.Address, input.Value));
                    case OscButtonType.SendId:
                        if (input.Value == 1)
                        {
                            return new MaybeOscMessage(new OscMessage(map.Address, map.Id));
                        }
                        return new MaybeOscMessage();
                }
            }
            throw new Exception("Button " + input.Id + " does not have a map.");
        }

        private MaybeOscMessage BuildFaderMessage(Input input)
        {
            if (input.Id <= faderCommandMap.Length)
            {
                var map = faderCommandMap[input.Id];
                return new MaybeOscMessage(new OscMessage(map, (int)input.Value*0.392));
            }
            return new MaybeOscMessage();
        }

        public static void SendCommands(List<OscMessage> messages, OscConnection connection)
        {
            foreach (var message in messages)
            {
                connection.Send(message);
            }
        }
    }

    public class MaybeOscMessage
    {
        public bool IsNull;
        public OscMessage Message;

        public MaybeOscMessage(OscMessage m)
        {
            Message = m;
            IsNull = false;
        }

        public MaybeOscMessage()
        {
            IsNull = true;
        }
    }

    public class OscButtonCommandMap
    {
        public OscButtonType Type;
        public string Address;
        public int? Id;

        public OscButtonCommandMap(string address)
        {
            Type = OscButtonType.FireOnly;
            Address = address;
        }

        public OscButtonCommandMap(string address, int id)
        {
            Type = OscButtonType.SendId;
            Address = address;
            Id = id;
        }
    }

    public enum OscButtonType
    {
        SendId, FireOnly
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
