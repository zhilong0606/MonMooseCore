
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CopyDllTool
{
    [XmlRoot("Config")]
    [Serializable]
    public class Config
    {
        [XmlElement]
        public string dllFolderPath;
        [XmlElement]
        public List<CopyItem> itemList = new List<CopyItem>();

        [Serializable]
        public class CopyItem
        {
            [XmlElement]
            public bool enabled = true;
            [XmlElement]
            public string dllName;
            [XmlElement]
            public List<string> destFolderPathList = new List<string>();
        }

        private static string m_configPath = "Config.xml";

        private static Config m_instance = null;

        public static Config instance
        {
            get { return m_instance; }
        }

        public static void LoadConfig()
        {
            using (FileStream stream = File.Open(m_configPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                XmlSerializer serialize = new XmlSerializer(typeof(Config));
                try
                {
                    m_instance = serialize.Deserialize(stream) as Config;
                }
                catch
                {
                    m_instance = new Config();
                }
            }
        }

        public static void SaveConfig()
        {
            using (FileStream stream = File.Open(m_configPath, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlSerializer serialize = new XmlSerializer(typeof(Config));
                serialize.Serialize(stream, m_instance);
            }
        }
    }

}