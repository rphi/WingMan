using System;
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
