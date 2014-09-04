using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace FileMonitor
{
    using KiVs = KeyValuePair<int, string>;
    using System.Xml;
    public delegate void MsgSendHandle(object sender, MsgEvent e);

    public class CJMonitor
    {
        public KiVs Status = new KiVs(0, "");
        public  List<CJFolder> folders= new List<CJFolder>();
        private XmlDocument xmlDoc;
        private XmlElement rootNode;
        public MsgSendHandle MsgSend;
        private string confFile = "FileMonitor.conf";

        public CJMonitor()
        {
            getConf();
        }
        private void getConf()
        {
            xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(confFile);
                rootNode = xmlDoc.DocumentElement;
            }
            catch (Exception)
            {
                this.Status = new KiVs(-1, "配置文件加载失败");
            }
            if (Status.Key < 0)
            {
                return;
            }
            XmlNodeList folderNodes ;
            folderNodes = rootNode.SelectNodes("Folder");
            foreach (XmlNode node in folderNodes)
            {
                string path = node.Attributes["path"].Value ;
                string filter = node.Attributes["filter"].Value;
                string[] fs = filter.Split('|');
                string standDir = node.Attributes["standDir"].Value;
                string backupDir = node.Attributes["backDir"].Value;
                this.AddFolder(path, fs.ToList<string>(),standDir,backupDir);
            }
        }
        public void setConf()
        {
            foreach (XmlNode node in rootNode.SelectNodes("Folder"))
            {
                rootNode.RemoveChild(node);
                //xmlDoc.RemoveChild(node);
            }
            foreach (CJFolder fold in folders)
            {
                XmlElement folderNode = xmlDoc.CreateElement("Folder");
                folderNode.SetAttribute("path", fold.DirPath);
                string filterTmp = "";
                foreach (string str in fold.Filter)
                {
                    if (filterTmp == "")
                    {
                        filterTmp = str;
                    }
                    else
                    {
                        filterTmp += "|" + str;
                    }
                }
                folderNode.SetAttribute("filter", filterTmp);
                folderNode.SetAttribute("standDir", fold.StandDir);
                folderNode.SetAttribute("backDir", fold.BackupDir);
                rootNode.AppendChild(folderNode);
            }
            xmlDoc.Save(confFile);
        }
        public void AddFolder(string path,List<string> filter,string standdir,string backupdir)
        {
            CJFolder newfolder = new CJFolder(path,filter,standdir,backupdir);
            newfolder.FileChangeEvent +=
                new FileSystemEventHandler(fileSystemWatcher_EventHandle_Change);
            newfolder.FileCreateEvent +=
                new FileSystemEventHandler(fileSystemWatcher_EventHandle_Create);
            newfolder.FileDeleteEvent +=
                new FileSystemEventHandler(fileSystemWatcher_EventHandle_Delete);
            newfolder.FileRenameEvent +=
                new RenamedEventHandler(fileSystemWatcher_EventHandle_Rename);
            folders.Add(newfolder);
            setConf();
        }

        private void fileSystemWatcher_EventHandle_Change(object sender, FileSystemEventArgs e)  //文件增删改时被调用的处理方法
        {
            string str = e.ChangeType.ToString();
            string DirName = Path.GetDirectoryName(str);
            if (MsgSend != null)
            {
                MsgEvent ev = new MsgEvent();
                ev.Type = e.ChangeType.ToString();
                ev.Msg.Add(e.Name.ToString());
                ev.Msg.Add(e.FullPath.ToString());
                MsgSend(this, ev);
            }
        }
        private void fileSystemWatcher_EventHandle_Create(object sender, FileSystemEventArgs e)  //文件增删改时被调用的处理方法
        {
            string str = e.ChangeType.ToString();
            string DirName = Path.GetDirectoryName(str);
            if (MsgSend != null)
            {
                MsgEvent ev = new MsgEvent();
                ev.Type = e.ChangeType.ToString();
                ev.Msg.Add(e.Name.ToString());
                ev.Msg.Add(e.FullPath.ToString());
                MsgSend(this, ev);
            }
        }
        private void fileSystemWatcher_EventHandle_Delete(object sender, FileSystemEventArgs e)  //文件增删改时被调用的处理方法
        {
            string str = e.ChangeType.ToString();
            string DirName = Path.GetDirectoryName(str);
            if (MsgSend != null)
            {
                MsgEvent ev = new MsgEvent();
                ev.Type = e.ChangeType.ToString();
                ev.Msg.Add(e.Name.ToString());
                ev.Msg.Add(e.FullPath.ToString());
                MsgSend(this, ev);
            }
        }
        private void fileSystemWatcher_EventHandle_Rename(object sender, RenamedEventArgs e)   //文件重命名时被调用的处理方法
        {
            string str = e.ChangeType.ToString();
            string DirName = Path.GetDirectoryName(str);
            if (MsgSend != null)
            {
                MsgEvent ev = new MsgEvent();
                ev.Type = e.ChangeType.ToString();
                ev.Msg.Add(e.OldName.ToString());
                ev.Msg.Add(e.OldName.ToString());
                ev.Msg.Add(e.Name.ToString());
                MsgSend(this, ev);
            }
        }
    }

    public class MsgEvent : EventArgs
    {
        public EventArgs EventMsg;
        public List<string> Msg =new List<string>();
        public string Type;
        public string ShowControl;
        public int Num=0;
        public MsgEvent(){}
        public MsgEvent(string msg)
        {
            this.Msg.Add(msg);
        }
    }
}
