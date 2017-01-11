using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WingMan.Osc;

namespace WingMan
{
    public class ConfigLibrary
    {
        public Dictionary<string, ArduinoConfigObject> ArduinoConfigs;
        // add dictionaries for other source types here if need be

        public ConfigLibrary()
        {
            ArduinoConfigs = new Dictionary<string, ArduinoConfigObject>();
        }

        public static ConfigLibrary CreateConfigLibrary()
        {
            // try to load from config file
            if (File.Exists(Application.StartupPath + "/configlibrary.json"))
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(Application.StartupPath + "/configlibrary.json");
                    var fileContents = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<ConfigLibrary>(fileContents);
                }
                finally
                {
                    reader?.Close();
                }
            }
            return new ConfigLibrary();
        }

        public bool AddArduinoConfigObject(ArduinoConfigObject o, bool overwrite = true)
        {
            if (ArduinoConfigs.ContainsKey(o.DeviceId))
            {
                if (overwrite)
                {
                    ArduinoConfigs[o.DeviceId] = o;
                    Save();
                    return true;
                }
                return false;
            }
            // no duplication, just add it
            ArduinoConfigs.Add(o.DeviceId, o);
            Save();
            return true;
        }

        private void Save()
        {
            TextWriter writer = null;
            try
            {
                var fileContents = JsonConvert.SerializeObject(this);
                writer = new StreamWriter(Application.StartupPath + "/configlibrary.json");
                writer.Write(fileContents);
            }
            finally
            {
                writer?.Close();
            }
        }
    }

    public class ArduinoConfigObject
    {
        public string Hardware;
        public string DeviceId;
        public int Faders;
        public int Buttons;
        public string Version;

        public OscButtonCommandMap[] ButtonMap { get; set; }
        public string[] FaderMap { get; set; }

        public ArduinoConfigObject(string deviceId, string hardware, int faders, int buttons, string ver)
        {
            Hardware = hardware;
            DeviceId = deviceId;
            Faders = faders;
            Buttons = buttons;
            Version = ver;
        }
    }
}
