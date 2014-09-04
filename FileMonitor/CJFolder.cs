using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileMonitor
{
    using KsVs = KeyValuePair<string, string>;
    using KiVs = KeyValuePair<int, string>;
    using System.Windows.Forms;
    using System.Threading;

    public class CJFolder
    {
        private string path;
        public string BackupDir, StandDir;
        public List<string> Filter = new List<string>();
        public string DirPath{
            get{ return path; }
            set{ 
                if (Directory.Exists(value))
                {
                    this.path = value;
                    this.fileWatcher.Path = value;
                }
                else 
                {
                    this.Status = 
                        new KeyValuePair<int, string>(-1, "目录路径错误");
                }
            }}
        public KiVs Status = new KiVs(0, "");
        public FileSystemWatcher fileWatcher
            = new FileSystemWatcher();
        public event FileSystemEventHandler
            FileCreateEvent, FileDeleteEvent, FileChangeEvent;
        public event RenamedEventHandler FileRenameEvent;

        public CJFolder(string dirPath)
        {
            this.DirPath = dirPath;
            init();
        }
        public CJFolder(string dirPath,List<string> filter,string standDir,string backupDir)
        {
            this.DirPath = dirPath;
            this.Filter = filter;
            this.BackupDir = backupDir;
            this.StandDir = standDir;
            init();
        }
        private void init()
        {
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size| NotifyFilters.LastWrite;   //设置文件的文件名、目录名及文件的大小改动会触发Changed事件
            fileWatcher.Created += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle_Create);  //绑定事件触发后处理数据的方法。
            fileWatcher.Deleted += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle_Delete);
            fileWatcher.Changed += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle_Change);
            fileWatcher.Renamed += new RenamedEventHandler(this.fileSystemWatcher_Renamed);
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.EnableRaisingEvents = true;  //启动监控
        }
        private void fileSystemWatcher_EventHandle_Change(object sender, FileSystemEventArgs e)  //文件增删改时被调用的处理方法
        {
            bool isMonitor = false;
            string relaFilePath = "";
            string fileTypeTmp;
            if (e.Name.LastIndexOf(".") < 0)
            {
                fileTypeTmp = ".*";
            }
            else
            {
                fileTypeTmp = e.Name.Substring(e.Name.LastIndexOf(".")).ToLower();
            }
            foreach (string str in Filter)
            {
                if (str == "*.*")
                {
                    isMonitor = true;
                    break;
                }
                if (str.Substring(str.LastIndexOf(".")) == fileTypeTmp)
                {
                    isMonitor = true;
                    break;
                }
            }
            if (isMonitor)
            {
                try
                {
                    this.fileWatcher.EnableRaisingEvents = false;
                    relaFilePath = e.FullPath.ToString().Remove(0, DirPath.Length);
                    int dirIndex = relaFilePath.LastIndexOf("\\");
                    if (dirIndex > 0)
                    {
                        string dirStrTmp = relaFilePath.Remove(dirIndex);
                        Directory.CreateDirectory(BackupDir + dirStrTmp);
                    }
                    File.Move(e.FullPath, BackupDir + relaFilePath+"-"+DateTime.Now.ToFileTime().ToString());
                }
                catch (Exception ex)
                {
                    this.Status = new KiVs(-2, ex.Message);
                }
                if (this.FileChangeEvent != null)
                {
                    this.FileChangeEvent(this, e);
                }
                if (File.Exists(e.FullPath))
                {
                    File.Delete(e.FullPath);
                }
                try
                {
                    File.Copy(StandDir + relaFilePath, e.FullPath);
                }
                catch (Exception)
                {
                    Thread t = new Thread(() => 
                    {
                        bool isNeedOver = true;
                        while (isNeedOver)
                        try
                        {
                            File.Copy(StandDir + relaFilePath, e.FullPath);
                            isNeedOver = false;
                        }
                        catch
                        {
                            Thread.Sleep(3000);
                        }
                    });
                    t.Start();
                }
                this.fileWatcher.EnableRaisingEvents = true;
            }
        }
        private void fileSystemWatcher_EventHandle_Create(object sender, FileSystemEventArgs e)  //文件增删改时被调用的处理方法
        {
            bool isMonitor = false;
            string relaFilePath = "";
            string fileTypeTmp = e.Name.Substring(e.Name.LastIndexOf(".")).ToLower();
            foreach (string str in Filter)
            {
                if (str == "*.*")
                {
                    isMonitor = true;
                    break;
                }
                if (str.Substring(str.LastIndexOf(".")) == fileTypeTmp)
                {
                    isMonitor = true;
                    break;
                }
            }
            if (isMonitor)
            {
                try
                {
                    this.fileWatcher.EnableRaisingEvents = false;
                    relaFilePath = e.FullPath.ToString().Remove(0, DirPath.Length);
                    int dirIndex = relaFilePath.LastIndexOf("\\");
                    if ( dirIndex> 0)
                    {
                        string dirStrTmp = relaFilePath.Remove(dirIndex);
                        Directory.CreateDirectory(BackupDir + dirStrTmp);
                    }
                    File.Move(e.FullPath, BackupDir + relaFilePath + "-" + DateTime.Now.ToFileTime().ToString());
                }
                catch (Exception ex)
                {
                    this.Status = new KiVs(-2, ex.Message);
                }
                if (this.FileChangeEvent != null)
                {
                    this.FileCreateEvent(this, e);
                }
                this.fileWatcher.EnableRaisingEvents = true;
            }
        }
        private void fileSystemWatcher_EventHandle_Delete(object sender, FileSystemEventArgs e)  //文件增删改时被调用的处理方法
        {
            bool isMonitor = false;
            string relaFilePath = "";
            string fileTypeTmp = e.Name.Substring(e.Name.LastIndexOf(".")).ToLower();
            foreach (string str in Filter)
            {
                if (str == "*.*")
                {
                    isMonitor = true;
                    break;
                }
                if (str.Substring(str.LastIndexOf(".")) == fileTypeTmp)
                {
                    isMonitor = true;
                    break;
                }
            }
            if (isMonitor)
            {
                try
                {
                    this.fileWatcher.EnableRaisingEvents = false;
                    relaFilePath = e.FullPath.ToString().Remove(0, DirPath.Length);
                    File.Copy(StandDir + relaFilePath, e.FullPath);
                }
                catch (Exception ex)
                {
                    this.Status = new KiVs(-2, ex.Message);
                }
                if (this.FileChangeEvent != null)
                {
                    this.FileDeleteEvent(this, e);
                }
                this.fileWatcher.EnableRaisingEvents = true;
            }
        }
        private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)   //文件重命名时被调用的处理方法
        {
            bool isMonitor = false;
            string relaFilePath = "";
            string fileTypeTmp = e.OldName.Substring(e.OldName.LastIndexOf(".")).ToLower();
            foreach (string str in Filter)
            {
                if (str == "*.*")
                {
                    isMonitor = true;
                    break;
                }
                if (str.Substring(str.LastIndexOf(".")) == fileTypeTmp)
                {
                    isMonitor = true;
                    break;
                }
            }
            if (isMonitor)
            {
                try
                {
                    this.fileWatcher.EnableRaisingEvents = false;
                    relaFilePath = e.OldFullPath.ToString().Remove(0, DirPath.Length);
                    File.Delete(e.FullPath);
                    File.Copy(StandDir + relaFilePath, e.OldFullPath);
                }
                catch (Exception ex)
                {
                    this.Status = new KiVs(-2, ex.Message);
                }
                if (this.FileChangeEvent != null)
                {
                    this.FileRenameEvent(this, e);
                }
                this.fileWatcher.EnableRaisingEvents = true;
            }

        }

        //private void setRenamedLogText(RenamedEventArgs e)  //更新UI界面
        //{
        //    ListViewItem lvi = new ListViewItem();
            
        //    lvi.SubItems.Add(e.OldName);   //受影响的文件的原名
        //    lvi.SubItems.Add(e.ChangeType.ToString());  //受影响的文件的改动类型（Rename）
        //    lvi.SubItems.Add(e.Name);   //受影响的文件的新名
        //    lvi.SubItems.Add(e.OldFullPath);     //受影响的文件的原路径
        //    lvi.SubItems.Add(e.FullPath);  //受影响的文件的完整路径（其实和原路径一样）
        //    //this.fileEventLog_lvw.Items.Add(lvi);
        //}
    }
}
