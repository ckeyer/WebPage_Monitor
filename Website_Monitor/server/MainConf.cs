using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Website_Monitor.server
{
    public static class MainConf
    {
        private const string confPath = "Config.xml";
        public const string XMLHeader = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
        private  static XmlDocument xmlDoc = new XmlDocument();
        private  static XmlElement rootNode;
        public CJLog log;

        public enum ConfigState : uint
        {
            OK = 0,     // 获取成功
            DEFAULT,  // 有错误，而且已经使用默认值
            WARING,    // 警告
            ERROR    // 严重错误
        }

        public static void init()
        {
            getConf();
        }

        // 获取配置信息
        private static ConfigState getConf()
        {
            try
            {
                xmlDoc.Load(confPath);
            }
            catch
            {
                ConfigState cstmp = initConfFile();
                if (cstmp > ConfigState.WARING)
                {
                    return cstmp;
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
                log.loging("读取链接配置文件出现错误", CJLog.LogLevel.COMMON_ERROR);
                return ConfigState.DEFAULT;
                //System.Environment.Exit(-1);
            }
            return ConfigState.OK;
        }
        // 初始化配置文件
        private static ConfigState initConfFile()
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
                log.loging("初始化链接配置文件出错，可能需要重新安装",
                    CJLog.LogLevel.FATAL_ERROR);
                return ConfigState.ERROR;
            }
            return ConfigState.OK;
        }

    }
}
