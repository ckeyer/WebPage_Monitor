using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Website_Monitor.server
{
    using KsVs = KeyValuePair<string, string>;
    using KiVs = KeyValuePair<int, string>;
    public static class MainConf
    {
        static KiVs Status { public get; private set; }
        static string DatabasePath {public get; public set
            {
                ;
            }
        }

        
        private const string confPath = "Config.xml";
        private const string XMLHeader = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
        private static XmlDocument xmlDoc = new XmlDocument();
        private static XmlElement rootNode;

        public static void init()
        {
            getConf();
        }
        // 获取配置信息
        private static void getConf()
        {
            try
            {
                xmlDoc.Load(confPath);
            }
            catch
            {
                initConfFile();
                if (Status.Key < 0)
                {
                    return;
                }
                xmlDoc.LoadXml(confPath);
            }
            try
            {
                rootNode = xmlDoc.DocumentElement;
                XmlNode node;
                node = rootNode.SelectSingleNode("Project");
                //Author = node.SelectSingleNode("Author").InnerText;
                
                node = rootNode.SelectSingleNode("Config");
                foreach (XmlNode userNode in node.SelectNodes("Server"))
                {
                    //ServerIp = IPAddress.Parse(userNode.SelectSingleNode("IPAddress").InnerText);
                    //ServerTcpPort = int.Parse(userNode.SelectSingleNode("PortTcp").InnerText);
                    //ServerUdpPort = int.Parse(userNode.SelectSingleNode("PortUdp").InnerText);
                    //TimeOut = int.Parse(userNode.SelectSingleNode("TimeOut").InnerText);
                    //MaxRepeatTimes = int.Parse(userNode.SelectSingleNode("MaxRepeatTimes").InnerText);
                    //MaxContextBytes = int.Parse(userNode.SelectSingleNode("MaxContextBytes").InnerText);
                }
            }
            catch (Exception)
            {
                Status = new KiVs(-1, "获取配置信息失败");
                return ;
                //System.Environment.Exit(-1);
            }
            
        }
        // 初始化配置文件
        private static void initConfFile()
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                string xmlFrame = @"
<cjstudio>
  <Project>
    <Name>CHCJ</Name>
    <Version>v1.0</Version>
    <Author>CJ_Studio</Author>
    <CreateTime>2014-5-7 下午</CreateTime>
    <UpdateTime>2014-5-11 下午</UpdateTime>
  </Project>
 <Config>
    <Server>
      <IPAddress>127.0.0.1</IPAddress>
      <PortTcp>2222</PortTcp>
      <PortUdp>3333</PortUdp>
      <TimeOut>5000</TimeOut>
      <MaxRepeatTimes>3</MaxRepeatTimes>
      <MaxContextBytes>2048</MaxContextBytes>
    </Server>
  </Config>
</cjstudio>";
                xmlDoc.LoadXml(XMLHeader + xmlFrame);

                XmlElement rootNode = xmlDoc.DocumentElement;
                rootNode.SetAttribute("name", "connect.conf");
                xmlDoc.Save(confPath);
            }
            catch (Exception)
            {
                Status = new KiVs(-2,"初始化配置文件失败");
            }
        }

    }
}
