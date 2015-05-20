using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SetTransactionScope
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> machineConfigPaths = GetMachineConfigPaths();
            foreach (var machineConfigPath in machineConfigPaths)
            {
                SetAllowExeDefinition(machineConfigPath);
                SetTransactionTimeout(machineConfigPath);
                
            }
        }

        private static void SetAllowExeDefinition(string machineConfigPath)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(machineConfigPath);
                XmlNode node =
                    doc.SelectSingleNode("/configuration/system.transactions/machineSettings");
                if (node == null)
                {
                    node = doc.SelectSingleNode("/configuration");
                    var systemTransactionNode = doc.CreateNode("element", "system.transactions", "");
                    systemTransactionNode.InnerXml = @"<machineSettings maxTimeout=""01:00:00"" />";
                    node.AppendChild(systemTransactionNode);
                }
                else { 
                    var attribute = node.Attributes["maxTimeout"];
                    attribute.Value = "01:00:00";
                }
                doc.Save(machineConfigPath);
            }
            catch{}
        }

        private static void SetTransactionTimeout(string machineConfigPath)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(machineConfigPath);
                XmlNode node =
                    doc.SelectSingleNode(
                        "/configuration/configSections/sectionGroup[@name='system.transactions']/section[@name='machineSettings']");
                var attribute = node.Attributes["allowExeDefinition"];
                attribute.Value = "MachineToApplication";
                doc.Save(machineConfigPath);
            }
            catch
            {
            }
        }

        private static List<string> GetMachineConfigPaths()
        {
            var configs = new List<string>();
            var bit32 = @"C:\Windows\Microsoft.NET\Framework\{0}\config\machine.config";
            var bit64 = @"C:\Windows\Microsoft.NET\Framework64\{0}\config\machine.config";

            var versions = new List<string>()
            {
                "v2.0.50727",
                "v4.0.30319",
            };
            foreach (var version in versions)
            {
                var path32 = string.Format(bit32, version);
                if (File.Exists(path32)) ;
                    configs.Add(path32);
                var path64 = string.Format(bit64, version);
                if (File.Exists(path64)) ;
                configs.Add(path64);
            }
            return configs;
        }
    }
}
