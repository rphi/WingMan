using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WingMan.Objects;

namespace WingMan.Osc
{
    public static class SourceFactory
    {
        public static ISource CreateSource(SourceMode m, object args)
        {
            switch (m)
            {
                case SourceMode.Arduino:
                    return new Arduino.Source((Objects.ArduinoSourceFactoryArgs) args);
                default:
                    throw new Exception("Invalid SourceMode");
            }

        }
    }
}
