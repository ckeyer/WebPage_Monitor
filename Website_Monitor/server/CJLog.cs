using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website_Monitor.server
{
    class CJLog
    {
        private string log_file_path = "../../user/my.log";
        private FileStream fs;
        public LogLevel level = LogLevel.DEFAULT;
        public uint model = (uint)LogModel.MESSAGE_BOX_SHOW;
        private TextBox text_box;
        private ListBox list_box;
        public bool auto_datetime = true;
        delegate void CallBackLogControl(string str);
        CallBackLogControl cbLogControl;
        public DelegateLogHandle LogEvent;

        public enum LogLevel : uint // 日志级别 
        {
            FATAL_ERROR = 0,     // 严重错误
            COMMON_ERROR,   // 一般性错误
            WARING,     // 警告性信息
            IMPORTANT_MSG,  // 重要消息
            COMMON_MSG,     // 一般消息
            DEFAULT,    // 默认级别
            DEBUG,   // 调试
            ALL
        }
        public enum LogModel : uint // 日志记录方式 
        {
            NONE = 0,
            LOG_FILE = 1,     // 写入日志文件
            MESSAGE_BOX_SHOW = 2,     // 消息框弹出
            TEXT_BOX_SHOW = 4,    // TextBox 显示
            LIST_BOX_SHOW = 8     // ListBox 显示
        }

        public CJLog()
        {
            // fs = new FileStream(log_file_path, FileMode.OpenOrCreate);
        }
        public CJLog(string log_path)
        {
            setLogFilePath(log_path);
            model |= (uint)LogModel.LOG_FILE;
        }
        public CJLog(LogLevel lev, uint mod, string log_path)
        {
            setLogLevel(lev);
            setLogModel(mod);
            setLogFilePath(log_path);
            model |= (uint)LogModel.LOG_FILE;
        }
        public CJLog(LogLevel lev, uint mod)
        {
            setLogLevel(lev);
            setLogModel(mod);
        }
        public CJLog(LogLevel lev, LogModel mod)
        {
            setLogLevel(lev);
            setLogModel((uint)mod);
        }
        public CJLog(LogLevel lev, uint mod, Control cont)
        {
            setLogLevel(lev);
            setLogModel(mod);
            setControl(cont);
        }

        public void setControl(Control con)
        {
            if (con.GetType().ToString() == "System.Windows.Forms.ListBox")
            {
                list_box = (ListBox)con;
                cbLogControl = new CallBackLogControl(setListboxText);
            }
            else
            {
                text_box = (TextBox)con;
                cbLogControl = new CallBackLogControl(setTextboxText);
            }
        }
        public void setLogFilePath(string path)
        {
            if (fs != null)
            {
                fs.Close();
            }
            try
            {
                fs = new FileStream(path, FileMode.OpenOrCreate);
                log_file_path = path;
            }
            catch (Exception)
            {
                MessageBox.Show("日志文件配置错误", "CJ_Studio");
                fs = new FileStream(log_file_path, FileMode.OpenOrCreate);
            }
        }
        public void setLogModel(uint mod)
        {
            model = mod;
        }
        public void setLogModel(LogModel mod)
        {
            model = (uint)mod;
        }
        public void addLogModel(uint mod)
        {
            model |= mod;
        }
        public void setLogLevel(LogLevel lev)
        {
            level = lev;
        }

        public void loging(string str)
        {
            str = formatStr(str);
            if ((model & (uint)LogModel.LOG_FILE) != 0)
            {
                loging_logfile(str);
            }
            if ((model & (uint)LogModel.LIST_BOX_SHOW) != 0)
            {
                loging_listbox(str);
            }
            if ((model & (uint)LogModel.TEXT_BOX_SHOW) != 0)
            {
                loging_textbox(str);
            }
            if ((model & (uint)LogModel.MESSAGE_BOX_SHOW) != 0)
            {
                loging_messagebox(str);
            }
        }
        public void loging(string str, LogLevel lev)
        {
            // 待完善，分级处理
            if (lev <= this.level)
            {
                str = formatStr(str, lev);
                if ((model & (uint)LogModel.LOG_FILE) != 0)
                {
                    loging_logfile(str);
                }
                if ((model & (uint)LogModel.LIST_BOX_SHOW) != 0)
                {
                    loging_listbox(str);
                }
                if ((model & (uint)LogModel.TEXT_BOX_SHOW) != 0)
                {
                    loging_textbox(str);
                }
                if ((model & (uint)LogModel.MESSAGE_BOX_SHOW) != 0)
                {
                    loging_messagebox(str);
                }
            }
        }
        public void loging(byte[] str)
        {
            string s = System.Text.Encoding.UTF8.GetString(str);
            loging(s);
        }
        public void loging(byte[] str, LogLevel lev)
        {
            string s = System.Text.Encoding.UTF8.GetString(str);
            loging(s, lev);
        }

        private void loging_messagebox(string str)
        {
            MessageBox.Show(str, "CJ_Studio");
        }
        private void loging_textbox(string str)
        {
            text_box.Invoke(cbLogControl, str);
        }
        private void loging_listbox(string str)
        {
            list_box.Invoke(cbLogControl, str);
        }
        private void loging_logfile(string str)
        {
            if (fs == null)
            {
                try
                {
                    fs = new FileStream(log_file_path, FileMode.OpenOrCreate);
                }
                catch (Exception)
                {
                    MessageBox.Show("日志文件打开时出现严重错误", "CJ_Studio");
                }
            }
            fs.Seek(0, SeekOrigin.End);
            byte[] data = Encoding.UTF8.GetBytes(str + "\r\n");
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
        private string formatStr(string str)
        {
            if (!auto_datetime)
            {
                return str;
            }
            return string.Format("[{0}]: {1}", DateTime.Now.ToString(), str);
        }
        private string formatStr(string str, LogLevel msglev)
        {
            string strFormat = "[{1}]<{0}>: {2}";
            if (!auto_datetime)
            {
                strFormat = "[{1}]: {2}";
            }
            else
            {
                strFormat = "[{1}]<{0}>: {2}";
            }
            switch (msglev)
            {
                case LogLevel.FATAL_ERROR:
                    return string.Format(strFormat, DateTime.Now.ToString(), "严重错误", str);
                case LogLevel.COMMON_ERROR:
                    return string.Format(strFormat, DateTime.Now.ToString(), "错误", str);
                case LogLevel.WARING:
                    return string.Format(strFormat, DateTime.Now.ToString(), "警告", str);
                case LogLevel.IMPORTANT_MSG:
                    return string.Format(strFormat, DateTime.Now.ToString(), "重要消息", str);
                case LogLevel.COMMON_MSG:
                    return string.Format(strFormat, DateTime.Now.ToString(), "消息", str);
                case LogLevel.DEBUG:
                    return string.Format(strFormat, DateTime.Now.ToString(), "调试信息", str);
                case LogLevel.ALL:
                    return string.Format(strFormat, DateTime.Now.ToString(), "可忽略消息", str);
                default:
                    return string.Format(strFormat, DateTime.Now.ToString(), "消息", str);
            }
        }
        private void setTextboxText(string str)
        {
            text_box.Text += str;
            text_box.Text += "\r\n";
            text_box.SelectionStart = text_box.Text.Length - 1;
            text_box.ScrollToCaret();
        }
        private void setListboxText(string str)
        {
            list_box.Items.Add(str);
            list_box.SelectedIndex = list_box.Items.Count - 1;
        }

        public void logEvent(string str)
        {
            if (LogEvent != null)
            {
                CJLogEvent e = new CJLogEvent();
                e.logStr = formatStr(str);
                LogEvent(this, e);
            }
        }
        public void logEvent(string str, LogLevel lev)
        {
            if (LogEvent != null)
            {
                if (lev <= level)
                {
                    CJLogEvent e = new CJLogEvent();
                    e.logStr = formatStr(str, lev);
                    LogEvent(this, e);
                }
            }
        }
        public void logEvent(byte[] str)
        {
            string s = System.Text.Encoding.UTF8.GetString(str);
            logEvent(s);
        }
        public void logEvent(byte[] str, LogLevel lev)
        {
            string s = System.Text.Encoding.UTF8.GetString(str);
            logEvent(s, lev);
        }
    }
}
