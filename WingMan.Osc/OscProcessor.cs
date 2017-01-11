using Newtonsoft.Json;
using Rug.Osc;
using System;
using System.Collections.Generic;
using System.Net;
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
                var map = buttonCommandMap[input.Id - 1]; // zero based array, 1 based id
                if (map?.Address == "")
                {
                    return new MaybeOscMessage();
                }
                switch (map.Type)
                {
                    case OscButtonType.FireOnly:
                        return new MaybeOscMessage(new OscMessage(map.Address, input.Value));
                    case OscButtonType.SendData:
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
                var map = faderCommandMap[input.Id - 1]; // zero based array, 1 based id
                if (map == null) { return new MaybeOscMessage();}
                return new MaybeOscMessage(new OscMessage(map, input.Value));
            }
            return new MaybeOscMessage();
        }

        public static void SendCommands(List<OscMessage> messages, OscConnection connection)
        {
            connection.Send(messages);
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

        [JsonConstructor]
        public OscButtonCommandMap(OscButtonType type, string address, int? id)
        {
            switch (type)
            {
                case OscButtonType.FireOnly:
                    Type = OscButtonType.FireOnly;
                    Address = address;
                    break;
                case OscButtonType.SendData:
                    Type = OscButtonType.SendData;
                    Address = address;
                    Id = id;
                    break;
            }
        }

        public OscButtonCommandMap(string address)
        {
            Type = OscButtonType.FireOnly;
            Address = address;
        }

        public OscButtonCommandMap(string address, int id)
        {
            Type = OscButtonType.SendData;
            Address = address;
            Id = id;
        }
    }

    public enum OscButtonType
    {
        SendData, FireOnly
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

        public void Send(List<OscMessage> messages)
        {
            foreach (var message in messages)
            {
                Sender.Send(message);
            }
            Sender.WaitForAllMessagesToComplete();
        }
    }
}
