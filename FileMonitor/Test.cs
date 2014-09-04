using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;


namespace FileMonitor
{
    class Test
    {
        private FileSystemWatcher fileWatcher
            = new FileSystemWatcher();
        public event FileSystemEventHandler
            FileCreateEvent, FileDeleteEvent, FileChangeEvent;
        public void test()
        {
            //fileWatcher.Filter = "*.txt|*.doc|*.aspx";
            fileWatcher.Path = "F:\\Tmp";
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.NotifyFilter = 
                NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            fileWatcher.Changed += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
            fileWatcher.Created += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
            fileWatcher.Deleted += new FileSystemEventHandler(this.fileSystemWatcher_EventHandle);
            fileWatcher.Renamed += new RenamedEventHandler(this.fileSystemWatcher_Renamed);
            fileWatcher.EnableRaisingEvents = true;
        }
        private void fileSystemWatcher_EventHandle(object sender, FileSystemEventArgs e)  //文件增删改时被调用的处理方法
        {
            string s = e.FullPath.ToString();
            this.FileChangeEvent(this, e);
            //MessageBox.Show(e.ChangeType.ToString());
        }
        private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)  //文件增删改时被调用的处理方法
        {
            string s = e.FullPath.ToString();
            this.FileChangeEvent(this, e);
            //MessageBox.Show(e.ChangeType.ToString());
        }
    }
}
